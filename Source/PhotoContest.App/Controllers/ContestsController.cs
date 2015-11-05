namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Common;

    using CommonFunctions;

    using Data;
    using Data.Contracts;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.SignalR;

    using Models.Account;
    using Models.Contest;
    using Models.Pictures;

    using PagedList;

    using PhotoContest.App.Hubs;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    [System.Web.Mvc.Authorize]
    public class ContestsController : BaseController
    {
        public ContestsController(IPhotoContestData data)
            : base(data)
        {
            Mapper.CreateMap<User, BasicUserInfoViewModel>();
            Mapper.CreateMap<IPagedList<User>, IPagedList<BasicUserInfoViewModel>>()
                .ConvertUsing<PagedListConverter>();
        }

        // GET: Contests/Create
        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

        // POST: Contests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateContestBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            string thumbnailImageData = "";
            try
            {
                var image = PictureUtills.CreateImageFromBase64(model.Thumbnail);
                var thumbnail = PictureUtills.CreateThumbnailFromImage(image, 316);
                thumbnailImageData = PictureUtills.ConvertImageToBase64(thumbnail);
            }
            catch (Exception ex)
            {
                thumbnailImageData = model.Thumbnail;
            }

            var contest = new Contest()
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                OwnerId = loggedUserId,
                VotingType = model.VotingType,
                ParticipationType = model.ParticipationType,
                DeadlineType = model.DeadlineType,
                Picture = model.Thumbnail,
                Thumbnail = thumbnailImageData,
                Status = model.StartDate < DateTime.Now ? ContestStatus.Active : ContestStatus.Inactive,
            };

            if (contest.DeadlineType == DeadlineType.ParticipationLimit)
            {
                contest.ParticipationLimit = model.ParticipationLimit;
            }

            this.Data.Contests.Add(contest);
            this.Data.SaveChanges();

            if (model.VotingType == VotingType.Closed)
            {
                contest.Jury = new VotingCommittee();
                contest.Jury.ContestId = contest.Id;
            }

            foreach (var prize in model.Prizes)
            {
                var dbPrize = new Prize()
                {
                    Name = prize.Name,
                    Description = prize.Description,
                    ContestId = contest.Id,
                };
                contest.Prizes.Add(dbPrize);
            }

            this.Data.SaveChanges();

            var members = this.Data.Users.All().ToList();
            foreach (var member in members)
            {
                var notification = new Notification()
                {
                    RecipientId = member.Id,
                    Content = string.Format(
                        Messages.NewContest,
                        contest.Title,
                        contest.StartDate),
                    CreatedOn = DateTime.Now,
                    IsRead = false,
                };
                this.Data.Notifications.Add(notification);
            }
            this.Data.SaveChanges();

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ContestsHub>();
            hubContext.Clients.All.receiveMessage(contest.Id);

            return this.RedirectToAction("Contests", "Me");
        }

        [HttpGet]
        public ActionResult GetContestHubMessagePartial(int id)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new HttpRequestException(Messages.InvalidRequest);
            }

            var dbContest = this.Data.Contests.Find(id);
            var contest = Mapper.Map<ContestsHubNotificationViewModel>(dbContest);

            return this.PartialView("_ContestHubNotification", contest);
        }

        // GET: Contests/{contestId}
        // Returned model type: DetailsContestViewModel
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetContestById(int id)
        {
            var dbContest = this.Data.Contests.Find(id);
            var contest = Mapper.Map<DetailsContestViewModel>(dbContest);

            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            ICollection<ContestWinnerViewModel> contestWinners = null;
            if (dbContest.Status == ContestStatus.Finished)
            {
                contestWinners = this.GetContestWinners(dbContest);
            }

            if ((contest.Status == ContestStatus.Active || contest.Status == ContestStatus.Inactive) &&
                contest.EndDate.AddDays(1.0) < DateTime.Now)
            {
                dbContest.Status = ContestStatus.Finished;
                var notification = new Notification()
                {
                    Content = string.Format(
                        Messages.ContestFinished,
                        contest.Title),
                    RecipientId = dbContest.Owner.Id,
                    CreatedOn = DateTime.Now,
                    IsRead = false,
                };
                this.Data.Notifications.Add(notification);
                this.Data.SaveChanges();

                contestWinners = this.GetContestWinners(dbContest);

                foreach (var winner in contestWinners)
                {
                    var noty = new Notification()
                    {
                        Content = string.Format(
                            Messages.PictureWonPrize,
                            winner.Picture.Title ?? "(not available)",
                            winner.PrizeName,
                            contest.Title),
                        RecipientId = winner.WinnerId,
                        CreatedOn = DateTime.Now,
                        IsRead = false,
                    };
                    this.Data.Notifications.Add(notification);
                }
                this.Data.SaveChanges();
            }

            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (userId != null && dbContest.Owner.Id == userId)
            {
                contest.IsOwner = true;
            }

            if (userId != null && dbContest.ParticipationType == ParticipationType.Open ||
                dbContest.Participants.Any(p => p.Id == userId))
            {
                contest.CanParticipate = true;
            }

            if (userId != null && dbContest.ParticipationType == ParticipationType.Closed &&
                !dbContest.Candidates.Any(p => p.Id == userId) &&
                !dbContest.Participants.Any(p => p.Id == userId))
            {
                contest.CanApply = true;
            }

            var fullContestModel = new FullContestViewModel()
            {
                ContestSummary = contest,
                Winners = contestWinners,
            };

            return this.View(fullContestModel);
        }

        // GET: Contests/{contestId}/Manage
        [HttpGet]
        public ActionResult Manage(int id)
        {
            var contest = this.Data.Contests.All()
                .Where(c => c.Id == id).ProjectTo<EditContestBindingModel>().FirstOrDefault();

            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            if (contest.Status == ContestStatus.Finished || contest.Status == ContestStatus.Dismissed)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            if (contest.OwnerId != this.User.Identity.GetUserId() && !this.User.IsInRole("Administrator"))
            {
                return this.RedirectToAction("Contests", "Me");
            }

            return this.View(contest);
        }

        // POST: Contests/{contestId}/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(int id, EditContestBindingModel model)
        {
            if (model == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            contest.Title = model.Title;
            contest.Description = model.Description;
            contest.EndDate = model.EndDate;
            contest.StartDate = model.StartDate;
            contest.Thumbnail = model.Thumbnail;

            for (int i = 0; i < contest.Prizes.Count; i++)
            {
                contest.Prizes.ElementAt(i).Name = model.Prizes.ElementAt(i).Name;
                contest.Prizes.ElementAt(i).Description = model.Prizes.ElementAt(i).Description;
            }
            this.Data.SaveChanges();
            return this.RedirectToAction("GetContestById", "Contests", new { id = id });
        }

        // GET: Contests/{contestId}/Jury
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Jury(int id)
        {
            var contest = this.Data.Contests.All()
                .FirstOrDefault(c => c.Id == id);

            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var juryMembers = this.Data.Contests.All()
            .Where(c => c.Id == id).Select(c => c.Jury.Members).FirstOrDefault();

            var juryMembersView = Mapper.Map<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>(juryMembers);

            if (juryMembersView == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var juryViewModel = new JuryViewModel
            {
                Members = juryMembersView,
                ContestId = id
            };

            if (this.User.Identity.GetUserId() == contest.OwnerId)
            {
                juryViewModel.IsContestOwner = true;
            }
            else
            {
                juryViewModel.IsContestOwner = false;
            }

            this.ViewBag.ContestId = id;
            return this.View(juryViewModel);
        }

        // GET: Contests/{contestId}/AddJuryMember
        [HttpGet]
        public ActionResult AddJuryMember(int id)
        {
            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != contest.OwnerId)
            {
                return this.RedirectToAction("Contests", "Me");
            }

            var addJuryMemberBindingModel = new AddJuryMemberBindingModel
            {
                ContestId = id
            };
            return this.View(addJuryMemberBindingModel);
        }

        // POST: Contests/{contestId}/AddJuryMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJuryMember(AddJuryMemberBindingModel model)
        {
            if (this.User.Identity.GetUserName().ToLower() == model.Username.ToLower())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.OwnerCannotBeJurryMember);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == model.Username);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (user == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(model.ContestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Jury.Members.Any(u => u.Id == user.Id))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.UserAlreadyAddedToJurry);
                throw new System.Web.Http.HttpResponseException(message);
            }

            contest.Jury.Members.Add(user);
            this.Data.SaveChanges();

            return this.RedirectToRoute("Manage", new { action = "Jury", controller = "Contests", id = model.ContestId });
        }

        // POST: Contests/{contestId}/RemoveJuryMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveJuryMember(int id, string username)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (this.User.Identity.GetUserId() != contest.OwnerId && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAuhtorized);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (!contest.Jury.Members.Any(u => u.Id == user.Id))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.UserNotInJury);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var memberVotes = this.Data.Votes.All()
                .Where(v => v.ContestId == id && v.VoterId == user.Id);

            foreach (var vote in memberVotes)
            {
                this.Data.Votes.Delete(vote);
            }

            contest.Jury.Members.Remove(user);
            this.Data.SaveChanges();

            return this.Content(string.Empty);
        }

        [HttpGet]
        public ActionResult Apply(int contestId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());
            if (contest.Candidates.Any(u => u.Id == user.Id))
            {
                this.TempData["message"] = Messages.UserAlreadyAppliedToParticipate;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.Participants.Any(u => u.Id == user.Id))
            {
                this.TempData["message"] = Messages.UserAlreadyApprovedToParticipate;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            contest.Candidates.Add(user);
            var notification = new Notification()
            {
                Recipient = contest.Owner,
                CreatedOn = DateTime.Now,
                Content = string.Format("Member {0} applied to participate in the contest '{1}'. Please, go to contest page to process his/her application.",
                    user.UserName, contest.Title)
            };
            this.Data.Notifications.Add(notification);
            this.Data.SaveChanges();

            this.TempData["message"] = string.Format(Messages.SuccessfulyAppliedToContest, contest.Title);
            return this.RedirectToAction("GetContestById", new { id = contestId });
        }

        // GET: Contests/{contestId}/Candidates
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Candidates(int id, int? page)
        {
            var contestOwnerId = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.OwnerId)
                .FirstOrDefault();

            if (this.User.Identity.GetUserId() != contestOwnerId && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAuhtorized);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contestCandidates = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.Candidates)
                .FirstOrDefault()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            if (contestCandidates == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var candidatesView = Mapper.Map<IPagedList<User>, IPagedList<BasicUserInfoViewModel>>(contestCandidates);
            var candidatesViewModel = new CandidatesViewModel
            {
                Candidates = candidatesView,
                IsContestOwner = true,
                ContestId = id
            };

            return this.View(candidatesViewModel);
        }

        // POST: Contests/{contestId}/Candidates/ApproveCandidate/{username}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveCandidate(int id, string username)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.OwnerId != loggedUserId && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAuhtorized);
                throw new System.Web.Http.HttpResponseException(message);
            }

            contest.Participants.Add(user);
            contest.Candidates.Remove(user);
            this.Data.SaveChanges();

            return this.Content(string.Empty);
        }

        // POST: Contests/{contestId}/Candidates/RejectCandidate/{username}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectCandidate(int id, string username)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.OwnerId != loggedUserId && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAuhtorized);
                throw new System.Web.Http.HttpResponseException(message);
            }

            contest.Candidates.Remove(user);
            this.Data.SaveChanges();

            return this.Content(string.Empty);
        }

        // GET: Contests/{contestId}/Participants
        [HttpGet]
        public ActionResult Participants(int id, int? page)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var contestOwnerId = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.OwnerId)
                .FirstOrDefault();

            if (contestOwnerId == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var participants = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.Participants)
                .FirstOrDefault()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            var pagedParticipants = Mapper.Map<IPagedList<User>, IPagedList<BasicUserInfoViewModel>>(participants);
            var participantsViewModel = new ParticipantsViewModel
            {
                ContestId = id,
                Participants = pagedParticipants
            };

            if (loggedUserId == contestOwnerId || this.User.IsInRole("Administrator"))
            {
                participantsViewModel.IsContestOwner = true;
            }
            else
            {
                participantsViewModel.IsContestOwner = false;
            }

            return this.View(participantsViewModel);
        }

        // POST: Contests/{contestId}/RemoveParticipant?username={username}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveParticipant(int id, string username)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var contestOwnerId = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.OwnerId)
                .FirstOrDefault();

            if (contestOwnerId != loggedUserId && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAuhtorized);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.UserNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (!contest.Participants.Any(u => u.Id == user.Id))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.UserIsNotParticipantOfContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var votesForDeleting = this.Data.Votes.All()
                .Where(v => v.ContestId == id && v.Picture.AuthorId == user.Id);

            foreach (var vote in votesForDeleting)
            {
                this.Data.Votes.Delete(vote);
            }

            var participantPictures = contest.Pictures.Where(p => p.AuthorId == user.Id);
            foreach (var picture in participantPictures)
            {
                contest.Pictures.Remove(picture);
            }

            contest.Participants.Remove(user);
            this.Data.SaveChanges();

            return this.Content(string.Empty);
        }

        // GET: Contests/{contestId}/Gallery/{pictureId}
        // Returned model type: DetailsPictureViewModel
        [HttpGet]
        public ActionResult Gallery(int id, int? pictureId)
        {
            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Pictures.Count() == 0)
            {
                return this.View(new GalleryViewModel() { CurrentPicture = null });
            }

            if (pictureId == null)
            {
                pictureId = contest.Pictures.FirstOrDefault().Id;
            }

            var picture = contest.Pictures.FirstOrDefault(p => p.Id == pictureId);
            if (picture == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NoSuchPictureInContest);
                throw new System.Web.Http.HttpResponseException(message);
            }
            var pictureModel = Mapper.Map<DetailsPictureViewModel>(picture);

            var user = this.Data.Users.Find(this.User.Identity.GetUserId());

            pictureModel.CanBeDeleted = PictureUtills.IsAuthor(user, picture) || this.User.IsInRole("Administrator");
            pictureModel.CanBeRemoved = pictureModel.CanBeDeleted;
            pictureModel.CanBeVoted = PictureUtills.CanVoteForPicture(user, picture, contest);
            pictureModel.ContestId = id;
            pictureModel.VotesCount = picture.Votes.Where(v => v.ContestId == id).Count();

            var idList = contest.Pictures.OrderBy(p => p.Id).Select(p => p.Id).ToList();
            int? previousPictureId = null;
            int? nextPictureId = null;

            int currentPictureIndex = 0;
            for (int i = 0; i < idList.Count; i++)
            {
                if (idList[i] == pictureId)
                {
                    currentPictureIndex = i;
                    previousPictureId = i == 0 ? (int?)null : idList[i - 1];
                    nextPictureId = i == idList.Count - 1 ? (int?)null : idList[i + 1];
                    break;
                }
            }

            var galleryModel = new GalleryViewModel()
            {
                ContestId = contest.Id,
                PicturesCount = idList.Count,
                CurrentPictureIndex = currentPictureIndex,
                PreviousPictureId = previousPictureId,
                NextPictureId = nextPictureId,
                CurrentPicture = pictureModel,
            };

            return this.View(galleryModel);
        }

        [HttpGet]
        public ActionResult Pictures(int id, int? page)
        {
            IPagedList<SummaryPictureViewModel> pictures = null;
            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var userId = this.User.Identity.GetUserId();
            var contestId = contest.Id;

            pictures = contest.Pictures
                .AsQueryable()
                .Include(p => p.Votes)
                .OrderByDescending(p => p.PostedOn)
                .Select(p => new SummaryPictureViewModel()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Author = p.Author.Name,
                    AuthorUsername = p.Author.UserName,
                    IsAuthor = p.AuthorId == userId,
                    ThumbnailImageData = p.PictureData,
                    ContestId = contest.Id,
                    AddToContest = false,
                    ContestsCount = p.Contests.Count,
                    VotesCount = p.Votes.Count,
                    ContestVotesCount = p.Votes.Where(v => v.ContestId == contestId).Count(),
                })
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            this.ViewData["ContestTitle"] = contest.Title;

            return this.View(pictures);
        }

        [HttpGet]
        public ActionResult SelectPictures(int id)
        {
            this.TempData["SelectPicture"] = true;
            this.TempData["contestId"] = id;

            return this.RedirectToAction("Pictures", "Me", new { contestId = id, addToContest = true });
        }

        [HttpGet]
        public ActionResult AddPicture(int contestId, int pictureId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Status != ContestStatus.Active)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.AddingPictureToInactiveContestNotAllowed);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var picture = this.Data.Pictures.Find(pictureId);
            if (picture == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.PictureNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var userId = this.User.Identity.GetUserId();
            if (picture.AuthorId != userId)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.AddingPictureNotAuthorNotAllowed);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Pictures.Any(p => p.Id == picture.Id))
            {
                this.TempData["message"] = Messages.PictureAlreadyAddedToContest;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.OwnerId == userId)
            {
                this.TempData["message"] = Messages.ModeratorNotAllowedToParticipate;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.ParticipationType == ParticipationType.Closed &&
                !contest.Participants.Any(p => p.Id == userId))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotAppliedOrApprovedToParticipateInContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            contest.Pictures.Add(picture);
            if (!contest.Participants.Any(p => p.Id == userId))
            {
                contest.Participants.Add(this.Data.Users.Find(userId));
            }
            this.Data.SaveChanges();
            this.CheckContestEndingCondition(contest);

            return this.RedirectToAction("GetContestById", new { id = contestId });
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int id, int pictureId)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            int votesCount = this.Vote(pictureId, id, true);

            var dbPicture = this.Data.Pictures.Find(pictureId);
            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);
            picture.VotesCount = votesCount;
            picture.CanBeVoted = false;
            picture.CanBeUnvoted = true;
            picture.ContestId = id;
            picture.CanBeDeleted = PictureUtills.IsAuthor(this.Data.Users.Find(this.User.Identity.GetUserId()), dbPicture) ||
                this.User.IsInRole("Administrator");
            picture.CanBeRemoved = picture.CanBeDeleted;

            return this.PartialView("_PictureInfo", picture);
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnVote(int id, int pictureId)
        {
            if (!this.Request.IsAjaxRequest())
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.InvalidOperation);
                throw new System.Web.Http.HttpResponseException(message);
            }

            int votesCount = this.Vote(pictureId, id, false);

            var dbPicture = this.Data.Pictures.Find(pictureId);
            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);
            picture.VotesCount = votesCount;
            picture.CanBeVoted = true;
            picture.CanBeUnvoted = false;
            picture.ContestId = id;
            picture.CanBeDeleted = PictureUtills.IsAuthor(this.Data.Users.Find(this.User.Identity.GetUserId()), dbPicture) ||
                this.User.IsInRole("Administrator");
            picture.CanBeRemoved = picture.CanBeDeleted;

            return this.PartialView("_PictureInfo", picture);
        }

        [HttpGet]
        public ActionResult Prizes(int contestId)
        {
            var prizes = this.Data.Prizes.All()
                .AsQueryable()
                .Where(p => p.ContestId == contestId)
                .ProjectTo<PrizeViewModel>()
                .ToList();

            if (prizes == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.NoPrizes);
                throw new System.Web.Http.HttpResponseException(message);
            }
            return this.View(prizes);
        }

        [HttpGet]
        public ActionResult Pause(int contestId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.OwnerId != this.User.Identity.GetUserId() && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotOwnerOfContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Status != ContestStatus.Active)
            {
                this.TempData["message"] = Messages.ContestCannotBePaused;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            contest.Status = ContestStatus.Inactive;
            var notification = new Notification()
            {
                CreatedOn = DateTime.Now,
                IsRead = false,
                Content = string.Format(Messages.ContestWasPaused, contest.Title),
                RecipientId = contest.OwnerId,
            };
            this.Data.Notifications.Add(notification);
            this.Data.SaveChanges();

            this.TempData["message"] = Messages.ContestPaused;
            return this.RedirectToAction("GetContestById", new { id = contestId });
        }

        [HttpGet]
        public ActionResult Restart(int contestId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.OwnerId != this.User.Identity.GetUserId() && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotOwnerOfContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Status != ContestStatus.Inactive)
            {
                this.TempData["message"] = Messages.ContestCannotBeRestarted;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.Status == ContestStatus.Inactive && contest.StartDate > DateTime.Now)
            {
                this.TempData["message"] = Messages.ContestWillStart;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            contest.Status = ContestStatus.Active;
            var notification = new Notification()
            {
                CreatedOn = DateTime.Now,
                IsRead = false,
                Content = string.Format(Messages.ContestWasRestarted, contest.Title),
                RecipientId = contest.OwnerId,
            };
            this.Data.Notifications.Add(notification);
            this.Data.SaveChanges();

            this.TempData["message"] = Messages.ContestRestarted;
            return this.RedirectToAction("GetContestById", new { id = contestId });
        }

        [HttpGet]
        public ActionResult Dismiss(int contestId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.OwnerId != this.User.Identity.GetUserId() && !this.User.IsInRole("Administrator"))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                message.Content = new StringContent(Messages.NotOwnerOfContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (contest.Status == ContestStatus.Dismissed)
            {
                this.TempData["message"] = Messages.ContestAlreadyDismissed;
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            contest.Status = ContestStatus.Dismissed;
            var notification = new Notification()
            {
                CreatedOn = DateTime.Now,
                IsRead = false,
                Content = string.Format(Messages.ContestWasDismissed, contest.Title),
                RecipientId = contest.OwnerId,
            };
            this.Data.Notifications.Add(notification);
            this.Data.SaveChanges();

            this.TempData["message"] = Messages.ContestDismissed;
            return this.RedirectToAction("GetContestById", new { id = contestId });
        }

        [HttpPost]
        public ActionResult GalleryVote(int pictureId, int contestId, bool vote = true)
        {
            int votesCount = 0;
            if (vote)
            {
                votesCount = this.Vote(pictureId, contestId, true);
            }
            else
            {
                votesCount = this.Vote(pictureId, contestId, false);
            }
            return this.Content(string.Format(Messages.TotalVotes, votesCount));
        }

        private ICollection<ContestWinnerViewModel> GetContestWinners(Contest contest)
        {
            List<ContestWinnerViewModel> winners = new List<ContestWinnerViewModel>();
            int winnersCount = contest.Prizes.Count() <= contest.Pictures.Count ?
                contest.Prizes.Count() : contest.Pictures.Count;

            var winningPictures = contest.Pictures
                .OrderByDescending(p => p.Votes.Where(v => v.ContestId == contest.Id).Count())
                .Take(winnersCount)
                .ToList();
            for (int i = 0; i < winningPictures.Count; i++)
            {
                var contestWinner = new ContestWinnerViewModel()
                {
                    PictureId = winningPictures[i].Id,
                    ContestId = contest.Id,
                    WinnerName = winningPictures[i].Author.Name,
                    WinnerUsername = winningPictures[i].Author.UserName,
                    WinnerId = winningPictures[i].Author.Id,
                    PrizeName = contest.Prizes[i].Name,
                    Picture = Mapper.Map<SummaryPictureViewModel>(winningPictures[i])
                };
                winners.Add(contestWinner);
            }

            return winners;
        }

        private void CheckContestEndingCondition(Contest contest)
        {
            if (contest.DeadlineType != DeadlineType.EndDate)
            {
                return;
            }

            if (contest.Pictures.Count == contest.ParticipationLimit)
            {
                contest.Status = ContestStatus.Finished;
                this.Data.SaveChanges();
            }
        }

        private int Vote(int pictureId, int contestId, bool vote = true)
        {
            var picture = this.Data.Pictures.Find(pictureId);
            if (picture == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.NoSuchPicture);
                throw new System.Web.Http.HttpResponseException(message);
            }

            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.ContestNotFound);
                throw new System.Web.Http.HttpResponseException(message);
            }

            if (!contest.Pictures.Any(p => p.Id == pictureId))
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(Messages.NoSuchPictureInContest);
                throw new System.Web.Http.HttpResponseException(message);
            }

            int votes = picture.Votes.Where(v => v.ContestId == contestId).Count();

            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            if (picture.AuthorId == user.Id)
            {
                return votes;
            }

            if (contest.Status != ContestStatus.Active)
            {
                return votes;
            }

            if (contest.VotingType == VotingType.Closed)
            {
                if (!contest.Jury.Members.Any(m => m.Id == userId))
                {
                    return votes;
                }
            }

            if (userId == contest.OwnerId)
            {
                return votes;
            }

            if (vote)
            {
                if (picture.Votes.Any(v => v.ContestId == contestId && v.VoterId == userId))
                {
                    return votes;
                }

                this.Data.Votes.Add(new Vote
                {
                    ContestId = contestId,
                    PictureId = pictureId,
                    VoterId = userId
                });

                var notification = new Notification
                {
                    Content = string.Format(
                    Messages.PictureVoteNotification,
                    user.UserName,
                    picture.Title ?? Messages.NoTitle,
                    contest.Title),
                    CreatedOn = DateTime.Now,
                    IsRead = false,
                    RecipientId = picture.AuthorId,
                };
                this.Data.Notifications.Add(notification);

                this.Data.SaveChanges();
                return ++votes;
            }
            else
            {
                var dbVote = picture.Votes.FirstOrDefault(v => v.ContestId == contestId && v.VoterId == userId);
                if (dbVote == null)
                {
                    return votes;
                }

                this.Data.Votes.Delete(dbVote);
                this.Data.SaveChanges();

                return --votes;
            }
        }
    }
}
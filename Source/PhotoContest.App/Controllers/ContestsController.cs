namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Common;
    using CommonFunctions;

    using Data.Contracts;

    using Microsoft.AspNet.Identity;

    using Models.Account;
    using Models.Contest;

    using Models.Pictures;
    using PagedList;
    
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    [Authorize]
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
            var loggedUserId = this.User.Identity.GetUserId();
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
                Thumbnail = model.Thumbnail,
                Status = model.StartDate < DateTime.Now ? ContestStatus.Active : ContestStatus.Inactive,
            };

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

            return this.RedirectToAction("Contests", "Me");
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
                return this.HttpNotFound();
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


            ICollection<ContestWinnerViewModel> contestWinners = null;
            if (dbContest.Status == ContestStatus.Finished)
            {
                this.GetContestWinners(dbContest);
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
                throw new ArgumentException("Contest not found!");
            }

            if (contest.OwnerId != this.User.Identity.GetUserId())
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
                throw new ArgumentException("Contest not found!");
            }

            var contest = this.Data.Contests.Find(id);
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
            return this.RedirectToAction("contests", "Me");
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
                throw new HttpRequestException("This contest does not exist!");
            }

            var juryMembers = this.Data.Contests.All()
            .Where(c => c.Id == id).Select(c => c.Jury.Members).FirstOrDefault();

            var juryMembersView = Mapper.Map<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>(juryMembers);

            if (juryMembersView == null)
            {
                return this.HttpNotFound();
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
                throw new HttpRequestException("This contest does not exist!");
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
                throw new ArgumentException("Owner of the contest can not be jury member!");
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == model.Username);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (user == null)
            {
                throw new ArgumentException("User not found!");
            }

            var contest = this.Data.Contests.Find(model.ContestId);
            if (contest == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (contest.Jury.Members.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException("This user has been already added as jury member!");
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
                throw new InvalidOperationException("Invalid operation!");
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                throw new ArgumentException("User not found!");
            }

            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (this.User.Identity.GetUserId() != contest.OwnerId)
            {
                throw new HttpRequestException("Not authorized!");
            }

            if (!contest.Jury.Members.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException("This user is not a jury member of this contest!");
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
                this.TempData["message"] = "You have already applied to participate in the contest.";
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.Participants.Any(u => u.Id == user.Id))
            {
                this.TempData["message"] = "You are already approved to participate in the contest.";
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            contest.Candidates.Add(user);
            //var notification = new Notification()
            //{
            //    Recipient = contest.Owner,
            //    CreatedOn = DateTime.Now,
            //    Content = string.Format("Member {0} applied to participate in the contest {1}. Please, go to contest page to process his/her application.",
            //        user.UserName, contest.Title)
            //};
            this.Data.SaveChanges();
            this.TempData["message"] = string.Format("Successfully applied to contest {0}.", contest.Title);
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

            if (this.User.Identity.GetUserId() != contestOwnerId)
            {
                throw new HttpRequestException("Not authorized!");
            }

            var contestCandidates = this.Data.Contests.All()
                .Where(c => c.Id == id)
                .Select(c => c.Candidates)
                .FirstOrDefault()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            if (contestCandidates == null)
            {
                throw new HttpRequestException("This contest not exists!");
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
                throw new InvalidOperationException("Invalid operation!");
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                throw new ArgumentException("User not exists!");
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                throw new ArgumentException("Contest not exists!");
            }

            if (contest.OwnerId != loggedUserId)
            {
                throw new HttpRequestException("Not authorized!");
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
                throw new InvalidOperationException("Invalid operation!");
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                throw new ArgumentException("User not exists!");
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                throw new ArgumentException("Contest not exists!");
            }

            if (contest.OwnerId != loggedUserId)
            {
                throw new HttpRequestException("Not authorized!");
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
                throw new HttpRequestException("Not existing contest!");
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

            if (loggedUserId == contestOwnerId)
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
                throw new InvalidOperationException("Invalid operation!");
            }

            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                throw new ArgumentException("User not found!");
            }

            var contest = this.Data.Contests.Find(id);
            if (contest == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (!contest.Participants.Any(u => u.Id == user.Id))
            {
                throw new ArgumentException("This user is not a participant of this contest!");
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
        public ActionResult Gallery(int contestId, int pictureId)
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult Pictures(int id, int? page)
        {
            IPagedList<SummaryPictureViewModel> pictures = null;
            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            pictures = contest.Pictures
                .AsQueryable()
                .OrderByDescending(p => p.PostedOn)
                .ProjectTo<SummaryPictureViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);

            for (int i = 0; i < pictures.Count; i++)
            {
                pictures[i].ContestId = id;
            }

            this.ViewData["ContestTitle"] = contest.Title;

            return View(pictures);
        }

        [HttpGet]
        public ActionResult SelectPictures(int id)
        {
            this.TempData["SelectPicture"] = true;
            this.TempData["contestId"] = id;

            return this.RedirectToAction("Pictures", "Me");
        }

        [HttpGet]
        public ActionResult AddPicture(int contestId, int pictureId)
        {
            var contest = this.Data.Contests.Find(contestId);
            if (contest == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var picture = this.Data.Pictures.Find(pictureId);
            if (picture == null)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            var userId = this.User.Identity.GetUserId();
            if (picture.AuthorId != userId)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            if (contest.Pictures.Any(p => p.Id == picture.Id))
            {
                this.TempData["message"] = "You have already added this picture to the contest.";
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.OwnerId == userId)
            {
                this.TempData["message"] = "Moderators are not allowed to participate in their contests.";
                return this.RedirectToAction("GetContestById", new { id = contestId });
            }

            if (contest.ParticipationType == ParticipationType.Closed &&
                !contest.Participants.Any(p => p.Id == userId))
            {
                throw new InvalidOperationException("You have not applied and/or have not been approved to participate in this contest.");
            }

            contest.Pictures.Add(picture);
            this.Data.SaveChanges();

            return this.RedirectToAction("GetContestById", new { id = contestId});
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int id, int pictureId)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (contest.VotingType == VotingType.Closed)
            {
                if (!contest.Jury.Members.Any(m => m.Id == loggedUserId))
                {
                    throw new ArgumentException("You are not allowed to vote only jury members can!");
                }
            }

            if (loggedUserId == contest.OwnerId)
            {
                throw new ArgumentException("You are not allowed to vote in your own contest!");
            }

            if (this.Data.Votes.All()
                .Any(
                v => v.PictureId == pictureId && 
                loggedUserId == v.VoterId &&
                id == contest.Id))
            {
                throw new ArgumentException("You are not allowed to vote more than one time each picture!");
            }
            
            this.Data.Votes.Add(new Vote
            {
                ContestId = id,
                PictureId = pictureId,
                VoterId = loggedUserId
            });

            this.Data.SaveChanges();

            var dbPicture = this.Data.Pictures.Find(pictureId);
            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);

            picture.HasVoted = true;
            picture.CanVote = false;
            picture.ContestId = id;

            var notification = new Notification
            {
                ContestId = id,
                PictureId = pictureId,
                CreatedOn = DateTime.Now,
                IsRead = false,
                NotificationType = NotificationType.Vote,
                SenderId = loggedUserId,
                RecipientId = dbPicture.AuthorId,
            };

            this.Data.Notifications.Add(notification);
            this.Data.SaveChanges();
            
            return this.PartialView("_PictureInfo", picture);
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UnVote(int id, int pictureId)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (loggedUserId == contest.OwnerId)
            {
                throw new ArgumentException("You are not allowed to vote in your own contest!");
            }

            var vote = this.Data.Votes.All()
                .FirstOrDefault(
                    v => v.PictureId == pictureId &&
                    v.ContestId == id &&
                    v.VoterId == loggedUserId);

            if (vote == null)
            {
                throw new ArgumentException("You have not voted to this picture in this contest!");
            }

            this.Data.Votes.Delete(vote);
            this.Data.SaveChanges();

            var dbPicture = this.Data.Pictures.Find(pictureId);
            var picture = Mapper.Map<DetailsPictureViewModel>(dbPicture);

            picture.HasVoted = false;
            picture.CanVote = true;
            picture.ContestId = id;

            return this.PartialView("_PictureInfo", picture);
        }

        private IEnumerable<ContestWinnerViewModel> GetContestWinners(Contest contest)
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
                    PrizeName = contest.Prizes[i].Name,
                    Picture = Mapper.Map<SummaryPictureViewModel>(winningPictures[i])
                };
                winners.Add(contestWinner);
            }

            return winners;
        }

    }
}
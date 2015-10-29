namespace PhotoContest.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.DynamicData;
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

            if (dbContest.Owner == user)
            {
                contest.IsOwner = true;
            }

            if (dbContest.ParticipationType == ParticipationType.Open ||
                dbContest.Participants.Any(p => p.Id == user.Id))
            {
                contest.CanParticipate = true;
            }

            return this.View(contest);
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

            if (contest.Owner.Id != this.User.Identity.GetUserId())
            {
                return this.RedirectToAction("Contests", "Me");
            }

            return this.View(contest);
        }

        // POST: Contests/{contestId}/Manage
        [HttpPost]
        public ActionResult Manage(int id, EditContestBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("Contest not found!");
            }

            if (model.Owner.Id != this.User.Identity.GetUserId())
            {
                return this.RedirectToAction("Contests", "Me");
            }

            var contest = this.Data.Contests.Find(id);
            contest.VotingType = model.VotingType;
            contest.DeadlineType = model.DeadlineType;
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

        // GET: Contests/{contestId}/Candidates
        // Returned model type: BasicUserInfoViewModel
        [HttpGet]
        public ActionResult Candidates(int? id)
        {
            if (id == null)
            {
                return this.HttpNotFound();
            }

            Mapper.CreateMap<User, BasicUserInfoViewModel>();

            var contest = this.Data.Contests.All().FirstOrDefault(c => c.Id == id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != this.User.Identity.GetUserId())
            {
                return this.HttpNotFound(); // TODO unauthorized!
            }

            var candidates = contest.Candidates;

            var candidatesView = Mapper.Map<IEnumerable<User>, IEnumerable<BasicUserInfoViewModel>>(candidates);

            if (candidatesView == null)
            {
                return this.HttpNotFound();
            }

            return this.View(candidatesView);
        }

        public ActionResult ManageCandidate(string operation, int id, string username)
        {
            if (operation == "approve")
            {
                return this.RedirectToAction("ApproveCandidate" , new {id, username});
            }

            return this.RedirectToAction("RejectCandidate", new { id, username });

        }

        // POST: Contests/{contestId}/Candidates/ApproveCandidate/{username}
        public ActionResult ApproveCandidate(int id, string username)
        {
            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();

            if (user == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != loggedUserId)
            {
                return this.HttpNotFound(); // TODO unauthorized;
            }

            contest.Participants.Add(user);
            contest.Candidates.Remove(user);
            this.Data.SaveChanges();

            return this.RedirectToAction("Contests", "Me");
        }

        // POST: Contests/{contestId}/Candidates/RejectCandidate/{username}
        public ActionResult RejectCandidate(int id, string username)
        {
            var user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
            var loggedUserId = this.User.Identity.GetUserId();
            if (user == null)
            {
                return this.HttpNotFound();
            }

            var contest = this.Data.Contests.Find(id);

            if (contest == null)
            {
                return this.HttpNotFound();
            }

            if (contest.OwnerId != loggedUserId)
            {
                return this.HttpNotFound(); // TODO unauthorized;
            }
            contest.Candidates.Remove(user);
            this.Data.SaveChanges();
            return this.RedirectToAction("Contests", "Me");
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

            contest.Pictures.Add(picture);
            this.Data.SaveChanges();

            return this.RedirectToAction("GetContestById", new { id = contestId});
        }



        // POST: Contests/{contestId}/Vote/{pictureId}
        [HttpPost]
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
            return this.Content(string.Empty);
        }

        // POST: Contests/{contestId}/Vote/{pictureId}
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

            return this.Content(string.Empty);
        }
    }
}
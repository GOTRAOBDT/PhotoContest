namespace PhotoContest.Common
{
    using System.Linq;

    using Microsoft.AspNet.Identity;

    using PhotoContest.Models;

    public class PictureUtills
    {
        public static bool CanVoteForPicture(User user, Picture dbPicture, Contest dbContest)
        {
            return PictureCanBeVotedUnvoted(user, dbPicture, dbContest) && !HasVotedForPicture(user, dbPicture, dbContest);
        }

        public static bool CanUnvotePicture(User user, Picture dbPicture, Contest dbContest)
        {
            return PictureCanBeVotedUnvoted(user, dbPicture, dbContest) && HasVotedForPicture(user, dbPicture, dbContest);
        }

        public static bool HasVotedForPicture(User user, Picture dbPicture, Contest dbContest)
        {
            if (dbPicture.Votes.Any(v => v.VoterId == user.Id && v.ContestId == dbContest.Id))
            {
                return true;
            }

            return false;
        }

        public static bool IsAuthor(User user, Picture dbPicture)
        {
            if (dbPicture.Author.Id ==user.Id)
            {
                return true;
            }

            return false;
        }

        private static bool PictureCanBeVotedUnvoted(User user, Picture dbPicture, Contest dbContest)
        {
            // 1. User is author
            if (dbPicture.AuthorId == user.Id)
            {
                return false;
            }

            // 2. User is contest owner
            if (dbContest.OwnerId == user.Id)
            {
                return false;
            }

            //3. Contest is not Active
            if (dbContest.Status != PhotoContest.Models.Enumerations.ContestStatus.Active)
            {
                return false;
            }

            // 4. Contest is voted by Jury and user is not member of the Jury
            if (dbContest.VotingType == PhotoContest.Models.Enumerations.VotingType.Closed &&
                !dbContest.Jury.Members.Any(m => m.Id == user.Id))
            {
                return false;
            }

            return true;
        }
    }
}

namespace PhotoContest.Common
{
    using System.Linq;

    using Microsoft.AspNet.Identity;

    using PhotoContest.Models;

    public class PictureUtills
    {
        public static bool CanVoteForPicture(User user, Picture dbPicture, Contest dbContest)
        {
            if (dbContest.Status != PhotoContest.Models.Enumerations.ContestStatus.Active)
            {
                return false;
            }

            if (dbPicture.Author == user)
            {
                return false;
            }

            if (dbContest.OwnerId == user.Id)
            {
                return false;
            }

            if (dbPicture.Votes.Any(v => v.VoterId == user.Id))
            {
                return false;
            }

            if (dbContest.VotingType == PhotoContest.Models.Enumerations.VotingType.Closed &&
                !dbContest.Jury.Members.Any(m => m.Id == user.Id))
            {
                return false;
            }

            return true;
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
    }
}

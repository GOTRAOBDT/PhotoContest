namespace PhotoContest.Common
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

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

        public static Image CreateImageFromBase64(string base64ImageData)
        {
            string pattern = "data:image/[^;]+;base64,";
            Regex rgx = new Regex(pattern);
            string formattedBase64Data = rgx.Replace(base64ImageData, "").Trim('\0');
            byte[] bytes = Convert.FromBase64String(formattedBase64Data);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public static string ConvertImageToBase64(Image imageToConvert)
        {
            byte[] byteData;
            using (var ms = new MemoryStream())
            {
                imageToConvert.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byteData =  ms.ToArray();
            }
            string result = Convert.ToBase64String(byteData);
            return "data:image/jpeg;base64," + result;
        }

        public static Image CreateThumbnailFromImage(Image image, int width)
        {
            Image resizedImage = image.GetThumbnailImage(width, (width * image.Height) / image.Width, null, IntPtr.Zero);
            return resizedImage;
        }
    }
}

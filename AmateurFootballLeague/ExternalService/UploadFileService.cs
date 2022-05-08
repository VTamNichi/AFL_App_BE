using Firebase.Auth;
using Firebase.Storage;
using System.Drawing;
using System.Drawing.Imaging;
using AmateurFootballLeague.Utils;

namespace AmateurFootballLeague.ExternalService
{
    public interface IUploadFileService
    {
        Task<string> UploadFile(IFormFile file, string bucket, string directory);
    }
    public class UploadFileService : IUploadFileService
    {
        private readonly IConfiguration _configuration;

        public UploadFileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFile(IFormFile file, string bucket, string directory)
        {
            var image = Image.FromStream(file.OpenReadStream());
            var resized = new Bitmap(image, new Size(300, 300));
            using var imageStream = new MemoryStream();
            resized.Save(imageStream, ImageFormat.Png);
            var imageBytes = imageStream.ToArray();
            MemoryStream memoryStream = new MemoryStream(imageBytes);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(FireBaseAppSetting.FirebaseApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(FireBaseAppSetting.FirebaseAuthEmail, FireBaseAppSetting.FirebaseAuthPassword);
            var task = new FirebaseStorage(
                FireBaseAppSetting.FirebaseBucket,
                new FirebaseStorageOptions()
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                });
            string fileExtension = Path.GetExtension(file.FileName);
            Guid guid = Guid.NewGuid();
            string fileName = guid.ToString() + "." + fileExtension;
            return await task.Child(bucket)
                .Child(directory)
                .Child(fileName)
                .PutAsync(memoryStream);
        }
    }
}

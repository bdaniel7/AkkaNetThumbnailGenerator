using System.IO;
using Akka.Actor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace AkkaNetThumbnailGenerator
{
	/// <summary>
	/// saves the thumbnail in a file
	/// </summary>
	public class WriteImageActor : ReceiveActor
	{
		readonly ActorSelection coordinatorActor;

		public WriteImageActor()
		{
			coordinatorActor = Context.System.ActorSelection("/user/startReadingPostsActor");

			Receive<WriteImage>(handleWriteImageMessage);
		}

		public class DoneWritingImage
		{
			public string PostPath { get; }

			public DoneWritingImage(string postPath)
			{
				PostPath = postPath;
			}
		}

		public class WriteImage
		{
			public WriteImage(Image image, string postPath)
			{
				Image = image;
				PostPath = postPath;
				FileName = Path.GetFileName(postPath) + ".png";
			}

			public string PostPath { get; }
			public Image Image { get; }
			public string FileName { get; }
		}

		void handleWriteImageMessage(WriteImage writeImage)
		{
			//FluentConsole.Red.Line($"writing image {writeImage.FileName}");

			using (var outputStream = File.OpenWrite(Path.Combine(Constants.OutputDirectory, writeImage.FileName)))
			{
				writeImage.Image.Save(outputStream, PngFormat.Instance);
			}

			writeImage.Image?.Dispose();

			coordinatorActor.Tell(new DoneWritingImage(writeImage.PostPath));
		}
	}
}
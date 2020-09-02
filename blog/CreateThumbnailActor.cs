using System;
using System.Globalization;
using Akka.Actor;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AkkaNetThumbnailGenerator
{
	/// <summary>
	/// creates thumbnail for a post
	/// </summary>
	public class CreateThumbnailActor : ReceiveActor
	{
		public CreateThumbnailActor()
		{
			Receive<CreateThumbnail>(handleCreateThumbnail);
		}

		public class CreateThumbnail
		{
			public string Title { get; }
			public string Author { get; }
			public DateTimeOffset? Date { get; }
			public string PostPath { get; }

			public CreateThumbnail(string title, string author,
			                              DateTimeOffset? date, string postPath)
			{
				Title = title;
				Author = author;
				Date = date;
				PostPath = postPath;
			}
		}

		void handleCreateThumbnail(CreateThumbnail createThumbnail)
		{
			// Image parameters
			var cardWidth = 876;
			var cardHeight = 438;
			var textPadding = 25;
			var titleSize = 42;
			var authorSize = 28;
			var titleLocation = new PointF(textPadding, cardHeight / 3.6f);
			var authorLocation = new PointF(textPadding, cardHeight / 4 + authorSize * 2);
			var font = Environment.OSVersion.Platform == PlatformID.Unix
					? SystemFonts.Find("DejaVu Sans")
					: SystemFonts.Find("Segoe UI");

			// Create image
			Image cardImage = new Image<Rgba32>(cardWidth, cardHeight);

			// Draw background image
			drawImage(cardImage, 0, 0, cardWidth, cardHeight, Image.Load(Constants.BackgroundImage));

			// Title
			drawText(cardImage, titleLocation.X,
													titleLocation.Y,
													cardWidth - textPadding - textPadding - textPadding - textPadding,
													Color.White, font.CreateFont(titleSize, FontStyle.Bold),
					createThumbnail.Title);

			// Author & date
			drawText(cardImage, authorLocation.X,
													authorLocation.Y,
													cardWidth - textPadding - textPadding - textPadding - textPadding,
													Color.White, font.CreateFont(authorSize, FontStyle.Italic),
					(createThumbnail.Author ?? "")
				+ (createThumbnail.Date?.ToString(" | MMMM dd, yyyy", CultureInfo.InvariantCulture) ?? ""));


			var writeImageActor = Context.ActorOf(Props.Create<WriteImageActor>());

			writeImageActor.Tell(new WriteImageActor.WriteImage(cardImage, createThumbnail.PostPath));
		}

		static void drawImage(Image image, float x, float y, int width, int height, Image other)
		{
			other.Mutate(ctx => ctx.Resize(width, height));

			image.Mutate(ctx => ctx.DrawImage(other, new Point(0, 0), opacity: 1f));
		}

		static void drawText(Image image, float x, float y, int width, Color color, Font font, string text)
		{
			var textGraphicsOptions = new TextGraphicsOptions();
			textGraphicsOptions.GraphicsOptions.Antialias = true;
			textGraphicsOptions.TextOptions.WrapTextWidth = width;

			var location = new PointF(x, y);

			image.Mutate(ctx => ctx.DrawText(textGraphicsOptions, text, font, color, location));
		}
	}
}
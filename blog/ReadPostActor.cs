using System.IO;
using Akka.Actor;
using YamlDotNet.Serialization;

namespace AkkaNetThumbnailGenerator
{
	/// <summary>
	/// reads one post
	/// </summary>
	public class ReadPostActor : ReceiveActor
	{
		readonly IDeserializer yamlDeserializer;

		public ReadPostActor(IDeserializer yamlDeserializer)
		{
			this.yamlDeserializer = yamlDeserializer;

			Receive<ReadNextPost>(handleReadPostMessage);
		}

		public class ReadNextPost
		{
			public ReadNextPost(string postPath)
			{
				PostPath = postPath;
			}
			public string PostPath { get; }
		}

		void handleReadPostMessage(ReadNextPost readNextPost)
		{
			//FluentConsole.Blue.Line($"reading post {readNextPost.PostPath}");

			var frontMatterYaml = File.ReadAllText(readNextPost.PostPath);
			if (frontMatterYaml == null) return;

			var temp = frontMatterYaml.Split("---");
			if (temp.Length < 2) return;

			var frontMatter = yamlDeserializer.Deserialize<FrontMatter>(temp[1]);

			// Cleanup front matter
			frontMatter.Title = frontMatter.Title.Replace("&amp;", "&");

			var createThumbnailActor = Context.ActorOf(Props.Create<CreateThumbnailActor>());

			createThumbnailActor.Tell(new CreateThumbnailActor.CreateThumbnail(frontMatter.Title,
																																								frontMatter.Author,
																																								frontMatter.Date,
																																								readNextPost.PostPath));

			}
	}
}
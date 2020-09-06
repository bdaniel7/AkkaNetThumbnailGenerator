using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using YamlDotNet.Serialization;

namespace AkkaNetThumbnailGenerator
{
	/// <summary>
	/// with router
	/// </summary>
	public class ReadingPostsCoordinatorActor : ReceiveActor
	{
		const int NR_OF_INSTANCES = 40;
		public IDeserializer YamlDeserializer { get; }
		public HashSet<string> PostPaths { get; }
		IActorRef readPostActor;
		int processedFiles;
		readonly Stopwatch stopwatch;
		readonly int totalPosts;

		public class ReadNextBatchOfPosts { }

		public ReadingPostsCoordinatorActor(IDeserializer yamlDeserializer, string[] postPaths)
		{
			YamlDeserializer = yamlDeserializer;
			PostPaths = postPaths.ToHashSet();
			totalPosts = postPaths.Length;
			initialize();

			stopwatch = new Stopwatch();
			stopwatch.Start();
		}

		/// <inheritdoc />
		protected override void PreStart()
		{
			readPostActor = Context.ActorOf(Props.Create<ReadPostActor>(YamlDeserializer)
			                                     .WithRouter(new RoundRobinPool(NR_OF_INSTANCES)), "readPost");
		}

		void initialize()
		{
			Receive<ReadNextBatchOfPosts>(sr => { handleReadNextBatchOfPosts(); });
			Receive<WriteImageActor.DoneWritingImage>(handleDoneWritingImage);
		}

		void handleReadNextBatchOfPosts()
		{
			var postPaths = PostPaths.Take(NR_OF_INSTANCES);

			foreach (var postPath in postPaths)
			{
				var readFirstPost = new ReadPostActor.ReadNextPost(postPath);

				readPostActor.Tell(readFirstPost);
			}
		}

		void handleDoneWritingImage(WriteImageActor.DoneWritingImage doneWritingImage)
		{
			PostPaths.Remove(doneWritingImage.PostPath);
			processedFiles++;

			if (processedFiles % NR_OF_INSTANCES == 0)
			{
				Self.Tell(new ReadNextBatchOfPosts());
			}

			if (processedFiles == totalPosts)
			{
				stopwatch.Stop();
				FluentConsole.Green.Line("Completed in {0}", stopwatch.Elapsed);
			}
		}
	}
}
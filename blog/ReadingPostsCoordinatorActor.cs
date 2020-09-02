using System.Collections.Generic;
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
		public IDeserializer YamlDeserializer { get; }
		public HashSet<string> PostPaths { get; }
		IActorRef readPostActor;

		public ReadingPostsCoordinatorActor(IDeserializer yamlDeserializer, string[] postPaths)
		{
			YamlDeserializer = yamlDeserializer;
			PostPaths = postPaths.ToHashSet();

			initialize();
		}

		/// <inheritdoc />
		protected override void PreStart()
		{
			readPostActor = Context.ActorOf(Props.Create<ReadPostActor>(YamlDeserializer)
			                                     .WithRouter(new RoundRobinPool(20)), "readPost");

		}

		void initialize()
		{
			Receive<StartReadingPosts>(sr => { handleReceiveStart(); });
			Receive<ReadPostActor.ReadNextPost>(handleReceiveReadPost);
			//Receive<WriteImageActor.DoneWritingImage>(handleFinishedReadingOnePost);
		}

		void handleReceiveStart()
		{
			var postPath = PostPaths.First();
			var readFirstPost = new ReadPostActor.ReadNextPost(postPath);

			readPostActor.Tell(readFirstPost);

			PostPaths.Remove(readFirstPost.PostPath);
		}

		void handleReceiveReadPost(ReadPostActor.ReadNextPost readPost)
		{
			// var props = Props.Create<ReadPostActor>(YamlDeserializer);
			// var readOnePostActor = Context.ActorOf(props);

			if (PostPaths.Count > 0)
			{
				var postPath = PostPaths.Take(1).Single();
				var readNextPost = new ReadPostActor.ReadNextPost(postPath);

				readPostActor.Tell(readNextPost);

				PostPaths.Remove(readPost.PostPath);

			}
		}

		// void handleFinishedReadingOnePost(WriteImageActor.DoneWritingImage finishedReading)
		// {
		// 	//Console.WriteLine($"finishedReadingMessage {finishedReadingMessage.PostPath}");
		// 	PostPaths.Remove(finishedReading.PostPath);
		//
		// 	var postPath = PostPaths.Take(1).Single();
		// 	var readNextPost = new ReadPostActor.ReadNextPost(postPath);
		//
		// 	Context.System.Scheduler.ScheduleTellOnce(0,
		// 			Self,
		// 			readNextPost,
		// 			Self);
		// }

		public class StartReadingPosts { }



		// /// <inheritdoc />
		// protected override void OnReceive(object message)
		// {
		// 	if (message is StartReadingPostsMessage startReadingPostsMessage)
		// 	{
		// 		var postPath = PostPaths.First();
		// 		var readFirstPostMessage = new ReadOnePostActor.ReadPostMessage(postPath);
		//
		// 		Context.System.Scheduler.ScheduleTellOnce(0,
		// 				Self,
		// 				readFirstPostMessage,
		// 				Self);
		// 	}
		//
		// 	if (message is ReadOnePostActor.ReadPostMessage readPostMessage)
		// 	{
		// 		var props = Props.Create<ReadOnePostActor>(YamlDeserializer);
		// 		var readOnePostActor = Context.ActorOf(props);
		// 		readOnePostActor.Tell(readPostMessage);
		//
		// 		PostPaths.Remove(readPostMessage.PostPath);
		//
		// 		if (PostPaths.Count > 0)
		// 		{
		// 			var postPath = PostPaths.Take(1).Single();
		// 			var readNextPostMessage = new ReadOnePostActor.ReadPostMessage(postPath);
		//
		// 			Context.System.Scheduler.ScheduleTellOnce(0,
		// 					Self,
		// 					readNextPostMessage,
		// 					Self);
		// 		}
		// 	}
		//
		// 	if (message is ReadOnePostActor.FinishedReadingOnePostMessage finishedReadingMessage)
		// 	{
		// 		//Console.WriteLine($"finishedReadingMessage {finishedReadingMessage.PostPath}");
		// 		PostPaths.Remove(finishedReadingMessage.PostPath);
		//
		// 		var postPath = PostPaths.Take(1).Single();
		// 		var readNextPostMessage = new ReadOnePostActor.ReadPostMessage(postPath);
		//
		// 		Context.System.Scheduler.ScheduleTellOnce(0,
		// 				Self,
		// 				readNextPostMessage,
		// 				Self);
		// 	}
		// }
	}
}
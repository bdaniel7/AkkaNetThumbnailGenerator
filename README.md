# AkkaNetThumbnailGenerator

My attempt to use Akka.NET actors to replicate the solution using Channels from here https://blog.maartenballiauw.be/post/2020/08/26/producer-consumer-pipelines-with-system-threading-channels.html

The project generates 445 png files from 445 .markdown files.

All 3 solutions ran on a laptop, with Windows 10 Pro, with
Intel Xeon CPU E3-1505M v6 @ 3.50GHz, 3500 Mhz, 4 cores, 8 Logical Processors,
with a Toshiba NVME disk THNSN51T02DU7 of 1 TB.


|**Solution**|**Results**|
|------------|-----------|
|Akka.NET|Completed in 00:00:32.6075130|
|Channels|Completed in 00:00:35.3226643|
|Channels with Open.ChannelExtensions|Completed in 00:00:33.3976113|
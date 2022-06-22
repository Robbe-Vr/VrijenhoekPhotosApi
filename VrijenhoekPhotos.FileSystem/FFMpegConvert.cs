using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;
using Xabe.FFmpeg;

namespace VrijenhoekPhotos.FileSystem
{
    internal static class FFMpegConvert
    {
        public static async Task<bool> ConvertToMp4(string inputFile, string outputFile, string type, bool? retry = null)
        {
            if (String.IsNullOrEmpty(inputFile) || String.IsNullOrEmpty(outputFile)) return false;

            string parameters;
            if (retry == true)
            {
                parameters = "-profile:v baseline -pix_fmt yuv420p -video_track_timescale 600";
            }
            else
            {
                switch (type)
                {
                    case ".avi":
                        parameters = "-profile:v baseline -pix_fmt yuv420p -video_track_timescale 600";
                        break;

                    default:
                        parameters = "";
                        break;
                }
            }

            try
            {
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFile);

                IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                    ?.SetCodec(VideoCodec.h264);

                IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                    ?.SetCodec(AudioCodec.aac);

                await FFmpeg.Conversions.New()
                    .AddParameter(parameters, ParameterPosition.PostInput)
                    .AddStream(audioStream, videoStream)
                    .SetOutput(outputFile)
                    .Start();
            }
            catch (Exception e)
            {
                if (retry == null && type != ".avi")
                {
                    Logger.Log("First conversion failed! Retrying with special settings! Error: " + e.Message);
                    return await ConvertToMp4(inputFile, outputFile, type, true);
                }
                Logger.Log("Error converting video to mp4! Error: " + e.Message);
                return false;
            }

            return true;
        }

        public static async Task<bool> GetThumbnailFromVideo(string inputFile, string outputFile)
        {
            if (String.IsNullOrEmpty(inputFile) || String.IsNullOrEmpty(outputFile)) return false;

            try
            {
                await (await FFmpeg.Conversions.FromSnippet.Snapshot(inputFile, outputFile, TimeSpan.FromSeconds(0))).Start();
            }
            catch (Exception e)
            {
                Logger.Log("Error taking thumbnail snapshot from video! Error: " + e.Message);
                return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyzukaRuGrabberCore.DataModels;
using NUnit.Framework;

namespace MyzukaRuGrabberCore.UnitTests
{
    [TestFixture]
    public class CoreTester
    {
        [Test]
        public void TryGrabAndParsePage_Album()
        {
            AlbumHeader expected_album_header = new AlbumHeader(
                "The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек (Score) (2004)", 
                "OST", 
                "Graeme Revell",
                "2004",
                "Саундтрек",
                22,
                "mp3",
                "Fluffy",
                "EarthMan",
                (String)null,
                new Uri("https://cs5.myzuka.fm/img/71/10705993/28306323.jpg"),
                new Uri("https://myzuka.fm/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Hroniki-Riddika-Saundtrek-Score-2004")
            );
            expected_album_header.Label = "Нет данных";
            expected_album_header.UploadDate = "19.03.2008";

            CommonDataBase result1 = Core.TryGrabAndParsePage(
                new Uri("https://myzuka.fm/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Hroniki-Riddika-Saundtrek-Score-2004"), 
                "Mozilla/5.0 (Windows; I; Windows NT 5.1; ru; rv:1.9.2.13) Gecko/20100101 Firefox/4.0", 
                true, 
                false,
                CancellationToken.None);
            
            Assert.NotNull(result1);
            Assert.IsInstanceOf(typeof(ParsedAlbum), result1);

            ParsedAlbum conv_result1 = (ParsedAlbum) result1;
            Assert.IsTrue(conv_result1.Header.Equals(expected_album_header));
            Assert.IsTrue(conv_result1.Songs.Count == 22);
            Assert.IsTrue(
                conv_result1.Songs.TrueForAll(
                    osh => osh.Artist.Equals("Graeme Revell", StringComparison.OrdinalIgnoreCase) && osh.Album.Equals
                        ("The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек (Score) (2004)", 
                        StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void TryGrabAndParsePage_Song()
        {
            OneSongHeader expected_song_header = new OneSongHeader(
                1, 
                "Graeme Revell - Vaako Conspiracy", 
                "Vaako Conspiracy", 
                "Graeme Revell",
                "The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек (Score)",
                "OST",
                "03:20",
                "7,78 Мб",
                "320 Кб/с",
                "mp3",
                "Fluffy",
                new Uri("https://cs5.myzuka.fm/img/71/10705993/28306323.jpg"),
                new Uri("https://myzuka.fm/Song/164181/Graeme-Revell-Vaako-Conspiracy"),
                true
            );
            expected_song_header.Rating = 1527;
            CommonDataBase result1 = Core.TryGrabAndParsePage(
                new Uri("https://myzuka.fm/Song/164181/Graeme-Revell-Vaako-Conspiracy"),
                "Mozilla/5.0 (Windows; I; Windows NT 5.1; ru; rv:1.9.2.13) Gecko/20100101 Firefox/4.0",
                true,
                false,
                CancellationToken.None);

            Assert.NotNull(result1);
            Assert.IsInstanceOf(typeof(ParsedSong), result1);

            ParsedSong conv_result1 = (ParsedSong)result1;
            Assert.IsTrue(conv_result1.Header.Equals(expected_song_header));
            Assert.IsTrue(conv_result1.AlbumLink == new Uri("https://myzuka.fm/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Hroniki-Riddika-Saundtrek-Score-2004"));
        }
    }
}

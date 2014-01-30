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
                "The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек [Score] (2004)", 
                "OST", 
                "Graeme Revell",
                "2004",
                "Саундтрек",
                22,
                "mp3",
                "Fluffy",
                "Lannna",
                "",
                new Uri("http://fp42.myzuka.ru/Download.aspx?lid=727400&mid=5258695&date=20140130203947&sum=20dd0e5d919bd6def71c53aed0b47cdd&name=&ic=False&cr=False&ex=.jpg&il=False"),
                new Uri("http://myzuka.ru/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Хроники-Риддика-Саундтрек-Score-2004")
            );

            ACommonData result1 = Core.TryGrabAndParsePage(
                new Uri("http://myzuka.ru/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Хроники-Риддика-Саундтрек-Score-2004"), 
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
                        ("The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек [Score] (2004)", 
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
                "The Chronicles Of Riddick - OST / Хроники Риддика - Саундтрек [Score]",
                "OST",
                "03:19",
                "5,31 Мб",
                "223 Кб/с",
                "mp3",
                "Fluffy",
                new Uri("http://fp42.myzuka.ru/Download.aspx?lid=727400&mid=5258695&date=20140130224847&sum=0afff71bffd5ff030dad549c1973575f&name=&ic=False&cr=False&ex=.jpg&il=False"),
                new Uri("http://myzuka.ru/Song/164181/Graeme-Revell-Vaako-Conspiracy"),
                true
            );

            ACommonData result1 = Core.TryGrabAndParsePage(
                new Uri("http://myzuka.ru/Song/164181/Graeme-Revell-Vaako-Conspiracy"),
                "Mozilla/5.0 (Windows; I; Windows NT 5.1; ru; rv:1.9.2.13) Gecko/20100101 Firefox/4.0",
                true,
                false,
                CancellationToken.None);

            Assert.NotNull(result1);
            Assert.IsInstanceOf(typeof(ParsedSong), result1);

            ParsedSong conv_result1 = (ParsedSong)result1;
            Assert.IsTrue(conv_result1.Header.Equals(expected_song_header));
            Assert.IsTrue(conv_result1.AlbumLink == new Uri("http://myzuka.ru/Album/12059/Graeme-Revell-The-Chronicles-Of-Riddick-Ost-Хроники-Риддика-Саундтрек-Score-2004"));
        }
    }
}

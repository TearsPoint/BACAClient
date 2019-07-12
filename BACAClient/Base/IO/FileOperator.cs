using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Base;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Base.Ex;

namespace Base.IO
{
    /// <summary>
    /// 包含和文件相关的常用功能。
    /// </summary>
    public static class FileOperator
    {
        static IDictionary<string, string> _contentTypes = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        static FileOperator()
        {
            _contentTypes[".*"] = "application/octet-stream";
            _contentTypes[".001"] = "application/x-001";
            _contentTypes[".301"] = "application/x-301";
            _contentTypes[".323"] = "text/h323";
            _contentTypes[".906"] = "application/x-906";
            _contentTypes[".907"] = "drawing/907";
            _contentTypes[".a11"] = "application/x-a11";
            _contentTypes[".acp"] = "audio/x-mei-aac";
            _contentTypes[".ai"] = "application/postscript";
            _contentTypes[".aif"] = "audio/aiff";
            _contentTypes[".aifc"] = "audio/aiff";
            _contentTypes[".aiff"] = "audio/aiff";
            _contentTypes[".anv"] = "application/x-anv";
            _contentTypes[".asa"] = "text/asa";
            _contentTypes[".asf"] = "video/x-ms-asf";
            _contentTypes[".asp"] = "text/asp";
            _contentTypes[".asx"] = "video/x-ms-asf";
            _contentTypes[".au"] = "audio/basic";
            _contentTypes[".avi"] = "video/avi";
            _contentTypes[".awf"] = "application/vnd.adobe.workflow";
            _contentTypes[".biz"] = "text/xml";
            _contentTypes[".bmp"] = "application/x-bmp";
            _contentTypes[".bot"] = "application/x-bot";
            _contentTypes[".c4t"] = "application/x-c4t";
            _contentTypes[".c90"] = "application/x-c90";
            _contentTypes[".cal"] = "application/x-cals";
            _contentTypes[".cat"] = "application/vnd.ms-pki.seccat";
            _contentTypes[".cdf"] = "application/x-netcdf";
            _contentTypes[".cdr"] = "application/x-cdr";
            _contentTypes[".cel"] = "application/x-cel";
            _contentTypes[".cer"] = "application/x-x509-ca-cert";
            _contentTypes[".cg4"] = "application/x-g4";
            _contentTypes[".cgm"] = "application/x-cgm";
            _contentTypes[".cit"] = "application/x-cit";
            _contentTypes[".class"] = "java/*";
            _contentTypes[".cml"] = "text/xml";
            _contentTypes[".cmp"] = "application/x-cmp";
            _contentTypes[".cmx"] = "application/x-cmx";
            _contentTypes[".cot"] = "application/x-cot";
            _contentTypes[".crl"] = "application/pkix-crl";
            _contentTypes[".crt"] = "application/x-x509-ca-cert";
            _contentTypes[".csi"] = "application/x-csi";
            _contentTypes[".css"] = "text/css";
            _contentTypes[".cut"] = "application/x-cut";
            _contentTypes[".dbf"] = "application/x-dbf";
            _contentTypes[".dbm"] = "application/x-dbm";
            _contentTypes[".dbx"] = "application/x-dbx";
            _contentTypes[".dcd"] = "text/xml";
            _contentTypes[".dcx"] = "application/x-dcx";
            _contentTypes[".der"] = "application/x-x509-ca-cert";
            _contentTypes[".dgn"] = "application/x-dgn";
            _contentTypes[".dib"] = "application/x-dib";
            _contentTypes[".dll"] = "application/x-msdownload";
            _contentTypes[".doc"] = "application/msword";
            _contentTypes[".docx"] = "application/vnd.ms-word.document";
            _contentTypes[".dot"] = "application/msword";
            _contentTypes[".drw"] = "application/x-drw";
            _contentTypes[".dtd"] = "text/xml";
            _contentTypes[".dwf"] = "Model/vnd.dwf";
            _contentTypes[".dwf"] = "application/x-dwf";
            _contentTypes[".dwg"] = "application/x-dwg";
            _contentTypes[".dxb"] = "application/x-dxb";
            _contentTypes[".dxf"] = "application/x-dxf";
            _contentTypes[".edn"] = "application/vnd.adobe.edn";
            _contentTypes[".emf"] = "application/x-emf";
            _contentTypes[".eml"] = "message/rfc822";
            _contentTypes[".ent"] = "text/xml";
            _contentTypes[".epi"] = "application/x-epi";
            _contentTypes[".eps"] = "application/x-ps";
            _contentTypes[".eps"] = "application/postscript";
            _contentTypes[".etd"] = "application/x-ebx";
            _contentTypes[".exe"] = "application/x-msdownload";
            _contentTypes[".fax"] = "image/fax";
            _contentTypes[".fdf"] = "application/vnd.fdf";
            _contentTypes[".fif"] = "application/fractals";
            _contentTypes[".fo"] = "text/xml";
            _contentTypes[".frm"] = "application/x-frm";
            _contentTypes[".g4"] = "application/x-g4";
            _contentTypes[".gbr"] = "application/x-gbr";
            _contentTypes[".gcd"] = "application/x-gcd";
            _contentTypes[".gif"] = "image/gif";
            _contentTypes[".gl2"] = "application/x-gl2";
            _contentTypes[".gp4"] = "application/x-gp4";
            _contentTypes[".hgl"] = "application/x-hgl";
            _contentTypes[".hmr"] = "application/x-hmr";
            _contentTypes[".hpg"] = "application/x-hpgl";
            _contentTypes[".hpl"] = "application/x-hpl";
            _contentTypes[".hqx"] = "application/mac-binhex40";
            _contentTypes[".hrf"] = "application/x-hrf";
            _contentTypes[".hta"] = "application/hta";
            _contentTypes[".htc"] = "text/x-component";
            _contentTypes[".htm"] = "text/html";
            _contentTypes[".html"] = "text/html";
            _contentTypes[".htt"] = "text/webviewhtml";
            _contentTypes[".htx"] = "text/html";
            _contentTypes[".icb"] = "application/x-icb";
            _contentTypes[".ico"] = "image/x-icon";
            //_contentTypes[".ico"] = "application/x-ico";
            _contentTypes[".iff"] = "application/x-iff";
            _contentTypes[".ig4"] = "application/x-g4";
            _contentTypes[".igs"] = "application/x-igs";
            _contentTypes[".iii"] = "application/x-iphone";
            _contentTypes[".img"] = "application/x-img";
            _contentTypes[".ins"] = "application/x-internet-signup";
            _contentTypes[".isp"] = "application/x-internet-signup";
            _contentTypes[".IVF"] = "video/x-ivf";
            _contentTypes[".java"] = "java/*";
            _contentTypes[".jfif"] = "image/jpeg";
            _contentTypes[".jpe"] = "image/jpeg";
            _contentTypes[".jpe"] = "application/x-jpe";
            _contentTypes[".jpeg"] = "image/jpeg";
            _contentTypes[".jpg"] = "image/jpeg";
            //_contentTypes[".jpg"] = "application/x-jpg";
            _contentTypes[".js"] = "application/x-javascript";
            _contentTypes[".jsp"] = "text/html";
            _contentTypes[".la1"] = "audio/x-liquid-file";
            _contentTypes[".lar"] = "application/x-laplayer-reg";
            _contentTypes[".latex"] = "application/x-latex";
            _contentTypes[".lavs"] = "audio/x-liquid-secure";
            _contentTypes[".lbm"] = "application/x-lbm";
            _contentTypes[".lmsff"] = "audio/x-la-lms";
            _contentTypes[".ls"] = "application/x-javascript";
            _contentTypes[".ltr"] = "application/x-ltr";
            _contentTypes[".m1v"] = "video/x-mpeg";
            _contentTypes[".m2v"] = "video/x-mpeg";
            _contentTypes[".m3u"] = "audio/mpegurl";
            _contentTypes[".m4e"] = "video/mpeg4";
            _contentTypes[".mac"] = "application/x-mac";
            _contentTypes[".man"] = "application/x-troff-man";
            _contentTypes[".math"] = "text/xml";
            _contentTypes[".mdb"] = "application/msaccess";
            _contentTypes[".mdb"] = "application/x-mdb";
            _contentTypes[".mfp"] = "application/x-shockwave-flash";
            _contentTypes[".mht"] = "message/rfc822";
            _contentTypes[".mhtml"] = "message/rfc822";
            _contentTypes[".mi"] = "application/x-mi";
            _contentTypes[".mid"] = "audio/mid";
            _contentTypes[".midi"] = "audio/mid";
            _contentTypes[".mil"] = "application/x-mil";
            _contentTypes[".mml"] = "text/xml";
            _contentTypes[".mnd"] = "audio/x-musicnet-download";
            _contentTypes[".mns"] = "audio/x-musicnet-stream";
            _contentTypes[".mocha"] = "application/x-javascript";
            _contentTypes[".movie"] = "video/x-sgi-movie";
            _contentTypes[".mp1"] = "audio/mp1";
            _contentTypes[".mp2"] = "audio/mp2";
            _contentTypes[".mp2v"] = "video/mpeg";
            _contentTypes[".mp3"] = "audio/mp3";
            _contentTypes[".mp4"] = "video/mpeg4";
            _contentTypes[".mpa"] = "video/x-mpg";
            _contentTypes[".mpd"] = "application/vnd.ms-project";
            _contentTypes[".mpe"] = "video/x-mpeg";
            _contentTypes[".mpeg"] = "video/mpg";
            _contentTypes[".mpg"] = "video/mpg";
            _contentTypes[".mpga"] = "audio/rn-mpeg";
            _contentTypes[".mpp"] = "application/vnd.ms-project";
            _contentTypes[".mps"] = "video/x-mpeg";
            _contentTypes[".mpt"] = "application/vnd.ms-project";
            _contentTypes[".mpv"] = "video/mpg";
            _contentTypes[".mpv2"] = "video/mpeg";
            _contentTypes[".mpw"] = "application/vnd.ms-project";
            _contentTypes[".mpx"] = "application/vnd.ms-project";
            _contentTypes[".mtx"] = "text/xml";
            _contentTypes[".mxp"] = "application/x-mmxp";
            _contentTypes[".net"] = "image/pnetvue";
            _contentTypes[".nrf"] = "application/x-nrf";
            _contentTypes[".nws"] = "message/rfc822";
            _contentTypes[".odc"] = "text/x-ms-odc";
            _contentTypes[".out"] = "application/x-out";
            _contentTypes[".p10"] = "application/pkcs10";
            _contentTypes[".p12"] = "application/x-pkcs12";
            _contentTypes[".p7b"] = "application/x-pkcs7-certificates";
            _contentTypes[".p7c"] = "application/pkcs7-mime";
            _contentTypes[".p7m"] = "application/pkcs7-mime";
            _contentTypes[".p7r"] = "application/x-pkcs7-certreqresp";
            _contentTypes[".p7s"] = "application/pkcs7-signature";
            _contentTypes[".pc5"] = "application/x-pc5";
            _contentTypes[".pci"] = "application/x-pci";
            _contentTypes[".pcl"] = "application/x-pcl";
            _contentTypes[".pcx"] = "application/x-pcx";
            _contentTypes[".pdf"] = "application/pdf";
            _contentTypes[".pdf"] = "application/pdf";
            _contentTypes[".pdx"] = "application/vnd.adobe.pdx";
            _contentTypes[".pfx"] = "application/x-pkcs12";
            _contentTypes[".pgl"] = "application/x-pgl";
            _contentTypes[".pic"] = "application/x-pic";
            _contentTypes[".pko"] = "application/vnd.ms-pki.pko";
            _contentTypes[".pl"] = "application/x-perl";
            _contentTypes[".plg"] = "text/html";
            _contentTypes[".pls"] = "audio/scpls";
            _contentTypes[".plt"] = "application/x-plt";
            _contentTypes[".png"] = "image/png";
            _contentTypes[".png"] = "application/x-png";
            _contentTypes[".pot"] = "application/vnd.ms-powerpoint";
            _contentTypes[".ppa"] = "application/vnd.ms-powerpoint";
            _contentTypes[".pTP"] = "application/x-pTP";
            _contentTypes[".pps"] = "application/vnd.ms-powerpoint";
            _contentTypes[".ppt"] = "application/vnd.ms-powerpoint";
            _contentTypes[".ppt"] = "application/x-ppt";
            _contentTypes[".pr"] = "application/x-pr";
            _contentTypes[".prf"] = "application/pics-rules";
            _contentTypes[".prn"] = "application/x-prn";
            _contentTypes[".prt"] = "application/x-prt";
            _contentTypes[".ps"] = "application/x-ps";
            _contentTypes[".ps"] = "application/postscript";
            _contentTypes[".ptn"] = "application/x-ptn";
            _contentTypes[".pwz"] = "application/vnd.ms-powerpoint";
            _contentTypes[".r3t"] = "text/vnd.rn-realtext3d";
            _contentTypes[".ra"] = "audio/vnd.rn-realaudio";
            _contentTypes[".ram"] = "audio/x-pn-realaudio";
            _contentTypes[".ras"] = "application/x-ras";
            _contentTypes[".rat"] = "application/rat-file";
            _contentTypes[".rdf"] = "text/xml";
            _contentTypes[".rec"] = "application/vnd.rn-recording";
            _contentTypes[".red"] = "application/x-red";
            _contentTypes[".rgb"] = "application/x-rgb";
            _contentTypes[".rjs"] = "application/vnd.rn-realsystem-rjs";
            _contentTypes[".rjt"] = "application/vnd.rn-realsystem-rjt";
            _contentTypes[".rlc"] = "application/x-rlc";
            _contentTypes[".rle"] = "application/x-rle";
            _contentTypes[".rm"] = "application/vnd.rn-realmedia";
            _contentTypes[".rmf"] = "application/vnd.adobe.rmf";
            _contentTypes[".rmi"] = "audio/mid";
            _contentTypes[".rmj"] = "application/vnd.rn-realsystem-rmj";
            _contentTypes[".rmm"] = "audio/x-pn-realaudio";
            _contentTypes[".rmp"] = "application/vnd.rn-rn_music_package";
            _contentTypes[".rms"] = "application/vnd.rn-realmedia-secure";
            _contentTypes[".rmvb"] = "application/vnd.rn-realmedia-vbr";
            _contentTypes[".rmx"] = "application/vnd.rn-realsystem-rmx";
            _contentTypes[".rnx"] = "application/vnd.rn-realplayer";
            _contentTypes[".rp"] = "image/vnd.rn-realpix";
            _contentTypes[".rTP"] = "audio/x-pn-realaudio-plugin";
            _contentTypes[".rsml"] = "application/vnd.rn-rsml";
            _contentTypes[".rt"] = "text/vnd.rn-realtext";
            _contentTypes[".rtf"] = "application/msword";
            _contentTypes[".rtf"] = "application/x-rtf";
            _contentTypes[".rv"] = "video/vnd.rn-realvideo";
            _contentTypes[".sam"] = "application/x-sam";
            _contentTypes[".sat"] = "application/x-sat";
            _contentTypes[".sdp"] = "application/sdp";
            _contentTypes[".sdw"] = "application/x-sdw";
            _contentTypes[".sit"] = "application/x-stuffit";
            _contentTypes[".slb"] = "application/x-slb";
            _contentTypes[".sld"] = "application/x-sld";
            _contentTypes[".slk"] = "drawing/x-slk";
            _contentTypes[".smi"] = "application/smil";
            _contentTypes[".smil"] = "application/smil";
            _contentTypes[".smk"] = "application/x-smk";
            _contentTypes[".snd"] = "audio/basic";
            _contentTypes[".sol"] = "text/plain";
            _contentTypes[".sor"] = "text/plain";
            _contentTypes[".spc"] = "application/x-pkcs7-certificates";
            _contentTypes[".spl"] = "application/futuresplash";
            _contentTypes[".spp"] = "text/xml";
            _contentTypes[".ssm"] = "application/streamingmedia";
            _contentTypes[".sst"] = "application/vnd.ms-pki.certstore";
            _contentTypes[".stl"] = "application/vnd.ms-pki.stl";
            _contentTypes[".stm"] = "text/html";
            _contentTypes[".sty"] = "application/x-sty";
            _contentTypes[".svg"] = "text/xml";
            _contentTypes[".swf"] = "application/x-shockwave-flash";
            _contentTypes[".tdf"] = "application/x-tdf";
            _contentTypes[".tg4"] = "application/x-tg4";
            _contentTypes[".tga"] = "application/x-tga";
            _contentTypes[".tif"] = "image/tiff";
            _contentTypes[".tif"] = "application/x-tif";
            _contentTypes[".tiff"] = "image/tiff";
            _contentTypes[".tld"] = "text/xml";
            _contentTypes[".top"] = "drawing/x-top";
            _contentTypes[".torrent"] = "application/x-bittorrent";
            _contentTypes[".tsd"] = "text/xml";
            _contentTypes[".txt"] = "text/plain";
            _contentTypes[".uin"] = "application/x-icq";
            _contentTypes[".uls"] = "text/iuls";
            _contentTypes[".vcf"] = "text/x-vcard";
            _contentTypes[".vda"] = "application/x-vda";
            _contentTypes[".vdx"] = "application/vnd.visio";
            _contentTypes[".vml"] = "text/xml";
            _contentTypes[".vpg"] = "application/x-vpeg005";
            _contentTypes[".vsd"] = "application/vnd.visio";
            _contentTypes[".vsd"] = "application/x-vsd";
            _contentTypes[".vss"] = "application/vnd.visio";
            _contentTypes[".vst"] = "application/vnd.visio";
            _contentTypes[".vst"] = "application/x-vst";
            _contentTypes[".vsw"] = "application/vnd.visio";
            _contentTypes[".vsx"] = "application/vnd.visio";
            _contentTypes[".vtx"] = "application/vnd.visio";
            _contentTypes[".vxml"] = "text/xml";
            _contentTypes[".wav"] = "audio/wav";
            _contentTypes[".wax"] = "audio/x-ms-wax";
            _contentTypes[".wb1"] = "application/x-wb1";
            _contentTypes[".wb2"] = "application/x-wb2";
            _contentTypes[".wb3"] = "application/x-wb3";
            _contentTypes[".wbmp"] = "image/vnd.wap.wbmp";
            _contentTypes[".wiz"] = "application/msword";
            _contentTypes[".wk3"] = "application/x-wk3";
            _contentTypes[".wk4"] = "application/x-wk4";
            _contentTypes[".wkq"] = "application/x-wkq";
            _contentTypes[".wks"] = "application/x-wks";
            _contentTypes[".wm"] = "video/x-ms-wm";
            _contentTypes[".wma"] = "audio/x-ms-wma";
            _contentTypes[".wmd"] = "application/x-ms-wmd";
            _contentTypes[".wmf"] = "application/x-wmf";
            _contentTypes[".wml"] = "text/vnd.wap.wml";
            _contentTypes[".wmv"] = "video/x-ms-wmv";
            _contentTypes[".wmx"] = "video/x-ms-wmx";
            _contentTypes[".wmz"] = "application/x-ms-wmz";
            _contentTypes[".wp6"] = "application/x-wp6";
            _contentTypes[".wpd"] = "application/x-wpd";
            _contentTypes[".wpg"] = "application/x-wpg";
            _contentTypes[".wpl"] = "application/vnd.ms-wpl";
            _contentTypes[".wq1"] = "application/x-wq1";
            _contentTypes[".wr1"] = "application/x-wr1";
            _contentTypes[".wri"] = "application/x-wri";
            _contentTypes[".wrk"] = "application/x-wrk";
            _contentTypes[".ws"] = "application/x-ws";
            _contentTypes[".ws2"] = "application/x-ws";
            _contentTypes[".wsc"] = "text/scriptlet";
            _contentTypes[".wsdl"] = "text/xml";
            _contentTypes[".wvx"] = "video/x-ms-wvx";
            _contentTypes[".xdp"] = "application/vnd.adobe.xdp";
            _contentTypes[".xdr"] = "text/xml";
            _contentTypes[".xfd"] = "application/vnd.adobe.xfd";
            _contentTypes[".xfdf"] = "application/vnd.adobe.xfdf";
            _contentTypes[".xhtml"] = "text/html";
            _contentTypes[".xls"] = "application/vnd.ms-excel";
            _contentTypes[".xls"] = "application/x-xls";
            _contentTypes[".xlw"] = "application/x-xlw";
            _contentTypes[".xml"] = "text/xml";
            _contentTypes[".xpl"] = "audio/scpls";
            _contentTypes[".xq"] = "text/xml";
            _contentTypes[".xql"] = "text/xml";
            _contentTypes[".xquery"] = "text/xml";
            _contentTypes[".xsd"] = "text/xml";
            _contentTypes[".xsl"] = "text/xml";
            _contentTypes[".xslt"] = "text/xml";
            _contentTypes[".xwd"] = "application/x-xwd";
            _contentTypes[".x_b"] = "application/x-x_b";
            _contentTypes[".x_t"] = "application/x-x_t";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Encoding GetTxtEncoding(byte[] data)
        {
            if ((data[0] == 0xfe) && (data[1] == 0xff))
            {
                return new UnicodeEncoding(true, true);
            }
            else if ((data[0] == 0xff) && (data[1] == 0xfe))
            {
                if ((data[2] != 0) || (data[3] != 0))
                {
                    return new UnicodeEncoding(false, true);
                }
                else
                {
                    return new UTF32Encoding(false, true);
                }
            }
            else if ((data[0] == 0xef) && (data[1] == 0xbb) && (data[2] == 0xbf))
            {
                return Encoding.UTF8;
            }
            else if ((data[0] == 0) && (data[1] == 0) && (data[2] == 0xfe) && (data[3] == 0xff))
            {
                return new UTF32Encoding(true, true);
            }
            else
            {
                return UnicodeEncoding.ASCII;
            }
        }

        /// <summary>
        /// 从一个文件中加载可序列化对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns> 
        public static object Read<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream fileStream = FileOperator.OpenAsReadOnly(filePath))
                    {
                        XmlSerializer sr = new XmlSerializer(typeof(T));
                        return sr.Deserialize(fileStream);
                    }
                }
                catch { }
            }
            else
            {

            }
            return null;
        }

        /// <summary>
        /// 从一个文件中字节系列
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ReadArray(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream stream = FileOperator.OpenAsReadOnly(filePath))
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
                catch { }
            }
            return new byte[0];
        }

        /// <summary>
        /// 将文件以只读方式读取。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenAsReadOnly(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// 返回文件的哈希码。文件不存在时返回一个空的数组。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetHash(string filePath)
        {
            if (!File.Exists(filePath))
                return new byte[0];

            using (HashAlgorithm hashAlg = HashAlgorithm.Create())
            {
                using (FileStream fs = OpenAsReadOnly(filePath))
                {
                    return hashAlg.ComputeHash(fs);
                }
            }
        }

        /// <summary>
        /// 复制流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

        /// <summary>
        /// 返回当前流压缩后的流
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MemoryStream Compressed(Stream source)
        {
            if (source == null) return null;

            MemoryStream stream = new MemoryStream();

            Compressed(source, stream);

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// 返回当前流解压后的流
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MemoryStream Decompressed(Stream source)
        {
            if (source == null) return null;

            MemoryStream stream = new MemoryStream();

            Decompressed(source, stream);

            return stream;
        }

        /// <summary>
        /// 返回当前流解压后的流
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MemoryStream Decompressed(byte[] data)
        {
            if (data == null || data.Length == 0) return null;

            MemoryStream stream = new MemoryStream();

            using (MemoryStream source = new MemoryStream(data))
            {
                Decompressed(source, stream);
            }

            return stream;
        }

        /// <summary>
        /// 将一个文件压缩为目标文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public static bool Compressed(string sourceFile, string targetFile)
        {
            if (!File.Exists(sourceFile)) return false;

            using (Stream target = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
            {
                using (Stream source = FileOperator.OpenAsReadOnly(sourceFile))
                {
                    Compressed(source, target);
                }
            }
            FileOperator.SetFileVersion(targetFile, FileOperator.GetFileVersion(sourceFile));
            return true;
        }

        /// <summary>
        /// 将源流压缩到目标流。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Compressed(Stream source, Stream target)
        {
            using (GZipStream gZipStream = new GZipStream(target, CompressionMode.Compress, true))
            {
                source.Position = 0;
                byte[] buffer = new byte[source.Length];
                int bytesRead = source.Read(buffer, 0, buffer.Length);

                gZipStream.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// 将一个文件解压为目标文件
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public static bool Decompressed(string sourceFile, string targetFile)
        {
            if (!File.Exists(sourceFile)) return false;

            using (Stream target = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
            {
                using (Stream source = FileOperator.OpenAsReadOnly(sourceFile))
                {
                    Decompressed(source, target);
                }
            }

            FileOperator.SetFileVersion(targetFile, FileOperator.GetFileVersion(sourceFile));
            return true;
        }

        /// <summary>
        /// 将一个数据包解压并存为目标文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public static bool Decompressed(byte[] data, string targetFile)
        {
            if (data == null || data.Length == 0) return false;

            using (Stream target = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
            {
                using (Stream source = new MemoryStream(data))
                {
                    Decompressed(source, target);
                }
            }

            return true;
        }

        /// <summary>
        /// 将一个流解压到目标流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Decompressed(Stream source, Stream target)
        {
            source.Position = 0;
            using (GZipStream zipStream = new GZipStream(source, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[4096];
                while (true)
                {
                    int bytesRead = zipStream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    target.Write(buffer, 0, bytesRead);
                }
            }
            target.Position = 0;
        }

        /// <summary>
        /// 获取文件版本。文件不存在时返回DateTime.MinValue
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DateTime GetFileVersion(string filePath)
        {
            if (File.Exists(filePath))
                return File.GetLastWriteTime(filePath);

            return DateTime.MinValue;
        }

        /// <summary>
        /// 设置文件版本。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="version"></param>
        public static void SetFileVersion(string filePath, DateTime version)
        {
            if (version == DateTime.MinValue) return;

            if (File.Exists(filePath))
            {
                File.SetCreationTime(filePath, version);
                File.SetLastWriteTime(filePath, version);
            }
        }

        /// <summary>
        /// 返回两个表示版本的时间值是否一致。精确到分钟。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsEqualsAsVersion(DateTime first, DateTime second)
        {
            TimeSpan span = first.Subtract(second);

            return span.TotalMinutes < 0.5 && span.TotalMinutes > -0.5;
        }

        private static string GetResourceManifestName(string resourceName, Assembly assembly)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(a => a.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 从组合体中获取指定名称的嵌入的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static T GetEmbeddedResource<T>(Assembly assembly, string resourceName) where T : class
        {
            string manifestName = GetResourceManifestName(resourceName, assembly);
            if (string.IsNullOrEmpty(manifestName)) return default(T);

            if (typeof(T) == typeof(Stream))
                return assembly.GetManifestResourceStream(manifestName) as T;

            using (Stream st = assembly.GetManifestResourceStream(manifestName))
            {
                st.Position = 0;
                if (typeof(T) == typeof(string))
                {
                    using (StreamReader reader = new StreamReader(st))
                    {
                        object text = reader.ReadToEnd();
                        return (T)text;
                    }
                }
                else
                {
                    XmlObjectSerializer serializer = ObjectEx.GetXmlObjectSerializer(typeof(T));
                    return serializer.ReadObject(st) as T;
                }
            }
        }

        /// <summary>
        /// 返回多个目录的共同的根目录。不存在时返回null。
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string GetRoot(params string[] paths)
        {
            int ix = -1;
            char c;
            bool done = false;
            while (!done)
            {
                ix++;
                c = char.MinValue;
                foreach (string item in paths)
                {
                    if (ix > item.Length - 1)
                    {
                        done = true;
                        break;
                    }

                    if (c == char.MinValue)
                        c = item[ix];
                    else if (c != item[ix])
                    {
                        done = true;
                        break;
                    }
                }
            }
            ix = paths[0].LastIndexOf(Path.DirectorySeparatorChar, ix);
            return paths[0].Left(ix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo(this Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentType(string fileName)
        {
            string extName = Path.GetExtension(fileName) ?? string.Empty;
            if (!extName.StartsWith(".")) extName = "." + extName;

            string ct;
            if (!_contentTypes.TryGetValue(extName, out ct))
            {
                return "text/html";
            }
            return ct;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetExtensions(string contentType)
        {
            return _contentTypes.Where(a => a.Value.StartsWith(contentType + "/", StringComparison.CurrentCultureIgnoreCase)).Select(a => a.Key).Distinct();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAudioExtensions()
        {
            return GetExtensions("audio");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetVideoExtensions()
        {
            yield return ".flv";
            foreach (var item in GetExtensions("video"))
            {
                yield return item;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetImageExtensions()
        {
            yield return ".bmp";
            yield return ".dib";
            foreach (var item in GetExtensions("image"))
            {
                yield return item;
            }
        }

        static string[] _unvalidChars = new string[] { "*", "?", "\\", "/" };

        /// <summary>
        /// 文件名格式化，除掉非法字符
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FormatAsFileName(string fileName)
        {
            foreach (var item in _unvalidChars)
            {
                fileName = fileName.Replace(item, string.Empty);
            }
            return fileName;
        }

        /// <summary>
        /// 删除指定的文件。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }
        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }
        /// <summary>
        /// 取得一个文本文件的编码方式。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
                fs.Close();
                return targetEncoding;
            }
            catch (Exception ex)
            {
                Loger.Log("", "", ex);
            }
            return defaultEncoding;
        }

        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding
                //Unicode {0xFF, 0xFE};
                //BE-Unicode {0xFE, 0xFF};
                //UTF8 = {0xEF, 0xBB, 0xBF};
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置      
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }

    }


}

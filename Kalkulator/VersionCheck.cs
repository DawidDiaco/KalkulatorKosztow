using System.Net;
using System.IO;
using System.Windows;

namespace Kalkulator
{
    class VersionCheck
    {
        private string version;
        public void CheckVersion()
        {
            ReadVersion("res/Version.txt");
            WebRequest rq = WebRequest.Create("https://raw.githubusercontent.com/DawidDiaco/KalkulatorKosztow/master/Kalkulator/res/Version.txt");
            rq.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse rp = (HttpWebResponse)rq.GetResponse();
            Stream st = rp.GetResponseStream();
            StreamReader sr = new StreamReader(st);
            string odpowiedz = sr.ReadToEnd();
            if (version != odpowiedz)
            {
                string link = "https://github.com/DawidDiaco/KalkulatorKosztow/releases";
                MessageBox.Show("Istnieje nowsza wersja: " + odpowiedz + ". \n " + link);            }
        }

        private void ReadVersion(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                version = sr.ReadToEnd();
            }
        }
    }
}

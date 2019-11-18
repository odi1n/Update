using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateProject
{
    public partial class Update_Forms : Form
    {
        private string Link, LinkLog, LinkSoft;
        private string NameVersion = "Version.txt", 
            NameLog = "Log.txt", 
            NameSoft = "Soft.rar";

        /// <summary>
        /// Версия которая по ссылке
        /// </summary>
        public string LinkVersion { get; set; }
        /// <summary>
        /// Версия проекта
        /// </summary>
        public string ProjectVersion { get; set; }

        /// <summary>
        /// Проверка на обновление
        /// </summary>
        /// <param name="link">Ссылка по которой делаем проверку на обновление</param>
        public Update_Forms(string link = null)
        {
            InitializeComponent();
            this.Link = link + "/" + NameVersion;
            this.LinkLog = link + "/"+ NameLog;
            this.LinkSoft = link + "/"+ NameSoft;
        }

        /// <summary>
        /// Загружаем данные в поля
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            LinkVersion = Task.Run(() => CheckUpdate(Link)).ContinueWith(x => x.Result).Result;//получаем версию с сайта
            ProjectVersion = ProductVersion;//версию нашего проекта

            if ( ProjectVersion != LinkVersion )//если не равны то открываем кнопку скачивания
                button2.Enabled = true;

            richTextBox1.Text = Task.Run(() => GetLog(LinkLog)).ContinueWith(x => x.Result).Result ;//выводим лог программы

            Save();//получаем информацию о файле, размер файла

            this.Text = "Обновление | new v" + LinkVersion;
            label6.Text = LinkVersion;
            label7.Text = ProjectVersion;
        }

        /// <summary>
        /// Установить новую версию
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        /// <summary>
        /// Закрытие формы
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Проверяем версию программы на сайте
        /// </summary>
        /// <param name="Link"></param>
        /// <returns></returns>
        private static async Task<string> CheckUpdate(string Link)
        {
            var requestByte = await WebReq.Request().DownloadStringTaskAsync(new Uri(Link));
            return requestByte;
        }

        /// <summary>
        /// Получить лог
        /// </summary>
        /// <param name="LinkLog">Ссылка на лог</param>
        /// <returns></returns>
        private async Task<string> GetLog(string LinkLog)
        {
            var requestByte = await WebReq.Request().DownloadStringTaskAsync(new Uri(LinkLog));
            return requestByte;
        }

        /// <summary>
        /// Сохранить/узнать информацию о файле
        /// </summary>
        /// <param name="ChechInfo"></param>
        private void Save(bool ChechInfo = false)
        {
            WebClient webClient = WebReq.Request();

            using ( webClient )
            {
                if ( ChechInfo == false )
                    webClient.DownloadStringTaskAsync(new Uri(LinkSoft));
                else
                    webClient.DownloadFileAsync(new Uri(LinkSoft), LinkSoft.Split('/').Last());

                webClient.DownloadProgressChanged += (o, args) =>
                {
                    if ( ChechInfo == false )
                    {
                        label8.Text = (WebReq.BytesToString(args.BytesReceived));
                        progressBar1.Maximum = (int)args.BytesReceived;
                    }
                    else
                    {
                        progressBar1.Value = (int)args.BytesReceived;
                    }
                };

                webClient.DownloadFileCompleted += (o, args) =>
                {
                    if ( ChechInfo )
                    {
                        MessageBox.Show("Нажмите OK для обновления приложения.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DownOk();
                    }
                };
            }
        }

        /// <summary>
        /// Если успешно скачали файл
        /// </summary>
        void DownOk()
        {
            Process.Start("Update.exe", $"{NameSoft} {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.exe");//передаем в консоль параметры архив который скачали и файл который запустим после распаковки архива
            Environment.Exit(0);
        }

        /// <summary>
        /// Делаем проверку на новое обновление. Если оно есть то true иначе false
        /// </summary>
        /// <returns></returns>
        public bool CheckUpdateProj()
        {
            try
            {
                var LinkVersion = Task.Run(() => CheckUpdate(Link)).ContinueWith(x => x.Result).Result;
                var ProjectVersion = this.ProductVersion;
                if ( ProjectVersion != LinkVersion )
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
    partial class WebReq
    {
        /// <summary>
        /// Отправка запросса
        /// </summary>
        /// <returns></returns>
        public static WebClient Request()
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            webClient.Headers.Add("Accept-Encoding", "gzip, deflate");
            webClient.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7,af;q=0.6");
            webClient.Headers.Add("Host", "i66008ta.beget.tech");
            webClient.Headers.Add("Upgrade-Insecure-Requests", "1");
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
            return webClient;
        }

        /// <summary>
        /// Байты переводим в строку
        /// </summary>
        /// <param name="byteCount">Количество байт</param>
        /// <returns></returns>
        public static String BytesToString(long byteCount)
        {
            string[] suf = { "Byt", "KB", "MB", "GB", "TB", "PB", "EB" }; //
            if ( byteCount == 0 )
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}

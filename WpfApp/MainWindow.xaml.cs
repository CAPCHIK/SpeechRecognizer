using Microsoft.Win32;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;
        private AudioCutter converter;
        private GoogleStorage storage;
        private Speech recognizer;
        private ComboBox cbox;

        public MainWindow()
        {
            InitializeComponent();
            converter = new AudioCutter();
            storage = new GoogleStorage("speechreconition-197119");
            recognizer = new Speech();

            cbox = new ComboBox
            {
                Background = Brushes.LightBlue,
                SelectedIndex = 89
            };
            languages
                .Select(I => I.Item1)
                .ToList()
                .ForEach(BI => cbox.Items.Add(BI));
            Language.Children.Add(cbox);
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".mp3",
                Filter = "MP3 files (*.mp3)|*.mp3"
            };
            var success = fileDialog.ShowDialog() ?? false;
            if (success)
            {
                fileName = fileDialog.FileName;
                FileNameBlock.Text = fileDialog.FileName;
            }
        }
        private async void Recognize_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Выберете файл для распознования!");
                return;

            }

            var language = languages[cbox.SelectedIndex].Item2;

            SetStatus("Обработка аудио перед загрузкой");
            var (outName, rate) = await converter.CompressAudio(File.OpenRead(fileName), SetPercents);
            SetPercents(0);
            SetStatus("Отправка файла в хранилище, для обработки");
            string link;
            using (var fileStream = File.OpenRead(outName))
            {
                link = await storage.UploadFile(fileStream, Path.GetFileName(outName), SetPercents);
            }
            SetPercents(0);
            File.Delete(outName);
            SetStatus("Распознование речи, распознанный текст будет выведет в поле справа");
            var text = await recognizer.LongRecognize(link, language, rate, SetPercents);
            TranslatedText.Text = text;
            SetStatus("Готово");
        }


        private void SetPercents(int percents)
        {
            Dispatcher.Invoke(() => ProgressBar.Value = percents);
        }
        private void SetStatus(string status) => NowStatus.Text = status;


        private List<(string, string)> languages = new List<(string, string)>
        {
            ( "Afrikaans (Suid-Afrika)", "af-ZA" ),
            ( "አማርኛ (ኢትዮጵያ)", "am-ET" ),
            ( "Հայ (Հայաստան)", "hy-AM" ),
            ( "Azərbaycan (Azərbaycan)", "az-AZ" ),
            ( "Bahasa Indonesia (Indonesia)", "id-ID" ),
            ( "Bahasa Melayu (Malaysia)", "ms-MY" ),
            ( "বাংলা (বাংলাদেশ)", "bn-BD" ),
            ( "বাংলা (ভারত)", "bn-IN" ),
            ( "Català (Espanya)", "ca-ES" ),
            ( "Čeština (Česká republika)", "cs-CZ" ),
            ( "Dansk (Danmark)", "da-DK" ),( "Deutsch (Deutschland)", "de-DE" ),
            ( "English (Australia)", "en-AU" ),( "English (Canada)", "en-CA" ),( "English (Ghana)", "en-GH" ),( "English (Great Britain)", "en-GB" ),( "English (India)", "en-IN" ),( "English (Ireland)", "en-IE" ),( "English (Kenya)", "en-KE" ),( "English (New Zealand)", "en-NZ" ),( "English (Nigeria)", "en-NG" ),( "English (Philippines)", "en-PH" ),( "English (South Africa)", "en-ZA" ),( "English (Tanzania)", "en-TZ" ),( "English (United States)", "en-US" ),( "Español (Argentina)", "es-AR" ),( "Español (Bolivia)", "es-BO" ),( "Español (Chile)", "es-CL" ),( "Español (Colombia)", "es-CO" ),( "Español (Costa Rica)", "es-CR" ),( "Español (Ecuador)", "es-EC" ),( "Español (El Salvador)", "es-SV" ),( "Español (España)", "es-ES" ),( "Español (Estados Unidos)", "es-US" ),( "Español (Guatemala)", "es-GT" ),( "Español (Honduras)", "es-HN" ),( "Español (México)", "es-MX" ),( "Español (Nicaragua)", "es-NI" ),( "Español (Panamá)", "es-PA" ),( "Español (Paraguay)", "es-PY" ),( "Español (Perú)", "es-PE" ),( "Español (Puerto Rico)", "es-PR" ),( "Español (República Dominicana)", "es-DO" ),( "Español (Uruguay)", "es-UY" ),( "Español (Venezuela)", "es-VE" ),( "Euskara (Espainia)", "eu-ES" ),( "Filipino (Pilipinas)", "fil-PH" ),( "Français (Canada)", "fr-CA" ),( "Français (France)", "fr-FR" ),( "Galego (España)", "gl-ES" ),( "ქართული (საქართველო)", "ka-GE" ),( "ગુજરાતી (ભારત)", "gu-IN" ),( "Hrvatski (Hrvatska)", "hr-HR" ),( "IsiZulu (Ningizimu Afrika)", "zu-ZA" ),( "Íslenska (Ísland)", "is-IS" ),( "Italiano (Italia)", "it-IT" ),( "Jawa (Indonesia)", "jv-ID" ),( "ಕನ್ನಡ (ಭಾರತ)", "kn-IN" ),( "ភាសាខ្មែរ (កម្ពុជា)", "km-KH" ),( "ລາວ (ລາວ)", "lo-LA" ),( "Latviešu (latviešu)", "lv-LV" ),( "Lietuvių (Lietuva)", "lt-LT" ),( "Magyar (Magyarország)", "hu-HU" ),( "മലയാളം (ഇന്ത്യ)", "ml-IN" ),( "मराठी (भारत)", "mr-IN" ),( "Nederlands (Nederland)", "nl-NL" ),( "नेपाली (नेपाल)", "ne-NP" ),( "Norsk bokmål (Norge)", "nb-NO" ),( "Polski (Polska)", "pl-PL" ),( "Português (Brasil)", "pt-BR" ),( "Português (Portugal)", "pt-PT" ),( "Română (România)", "ro-RO" ),( "සිංහල (ශ්රී ලංකාව)", "si-LK" ),( "Slovenčina (Slovensko)", "sk-SK" ),( "Slovenščina (Slovenija)", "sl-SI" ),( "Urang (Indonesia)", "su-ID" ),( "Swahili (Tanzania)", "sw-TZ" ),( "Swahili (Kenya)", "sw-KE" ),( "Suomi (Suomi)", "fi-FI" ),( "Svenska (Sverige)", "sv-SE" ),( "தமிழ் (இந்தியா)", "ta-IN" ),( "தமிழ் (சிங்கப்பூர்)", "ta-SG" ),( "தமிழ் (இலங்கை)", "ta-LK" ),( "தமிழ் (மலேசியா)", "ta-MY" ),( "తెలుగు (భారతదేశం)", "te-IN" ),( "Tiếng Việt (Việt Nam)", "vi-VN" ),( "Türkçe (Türkiye)", "tr-TR" ),( "Ελληνικά (Ελλάδα)", "el-GR" ),( "Български (България)", "bg-BG" ),( "Русский (Россия)", "ru-RU" ),( "Српски (Србија)", "sr-RS" ),( "Українська (Україна)", "uk-UA" ),( "हिन्दी (भारत)", "hi-IN" ),( "ไทย (ประเทศไทย)", "th-TH" ),
            ( "한국어 (대한민국)", "ko-KR" ),
            ( "國語 (台灣)", "cmn-Hant-TW" ),
            ( "廣東話 (香港)", "yue-Hant-HK" ),
            ( "日本語（日本）", "ja-JP" ),
            ( "普通話 (香港)", "cmn-Hans-HK" )
            ,( "普通话 (中国大陆)", "cmn-Hans-CN" )

        };
    }
}

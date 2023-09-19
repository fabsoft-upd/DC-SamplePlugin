using Document_Companion.PropertyGrid.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SampleDocCompPlugin
{
    public enum SampleChoices
    { 
        [System.ComponentModel.Description("Sample Choice A")]
        A,
        [System.ComponentModel.Description("Sample Choice B")]
        B,
        [System.ComponentModel.Description("Sample Choice C")]
        C
    }

    public class SamplePluginsSettings : System.ComponentModel.INotifyPropertyChanged
    {
        [ValueRange(1, 100)]
        [MaxControlWidth(200)]
        [DisplayName("Sample Plugin Number Value")]
        public int SampleNumber
        {
            get
            {
                return _SampleNumber;
            }
            set
            {
                _SampleNumber = value;
                OnPropertyChanged();
            }
        }
        private int _SampleNumber = 5;


        [DisplayName("Sample Folder Path")]
        [InfoToolTip("This is a sample of a browsable folder path")]
        [EditorType(typeof(Document_Companion.PropertyGrid.Editors.FolderSelectEditor))]
        public string SamplePath
        {
            get
            {
                return _SamplePath;
            }
            set
            {
                _SamplePath = value;
                OnPropertyChanged();
            }
        }
        private string _SamplePath = "";

        [DisplayName("Sample Note")]
        public string SampleNote
        {
            get
            {
                return _SampleNote;
            }
            set
            {
                _SampleNote = value;
                OnPropertyChanged();
            }
        }
        private string _SampleNote = "";

        [DisplayName("Sample Choice")]
        [MaxControlWidth(200)]
        public SampleChoices SampleChoice
        {
            get
            {
                return _SampleChoice;
            }
            set
            {
                _SampleChoice = value;
                OnPropertyChanged();
            }
        }
        private SampleChoices _SampleChoice = SampleChoices.A;

        [Browsable(false)]
        public string SampleHiddenProperty
        {
            get
            {
                return _SampleHiddenProperty;
            }
            set
            {
                _SampleHiddenProperty = value;
                OnPropertyChanged();
            }
        }
        private string _SampleHiddenProperty = "";


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }


        public static SamplePluginsSettings Load(string serialized)
        {
            SamplePluginsSettings instance = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(serialized))
                {
                    instance = Deserialize(serialized);
                }
            }
            catch (Exception ex)
            {
            }

            if (instance == null)
            {
                instance = new SamplePluginsSettings();
            }

            return instance;
        }

        private static XmlSerializer _Serializer = new XmlSerializer(typeof(SamplePluginsSettings));
        public static SamplePluginsSettings Deserialize(string serialized)
        {
            using (Stream ms = new System.IO.MemoryStream(Convert.FromBase64String(serialized)))
            {
                var newInstance = (SamplePluginsSettings)_Serializer.Deserialize(ms);
                return newInstance;
            }
        }

        public string Serialize()
        {
            using (Stream ms = new System.IO.MemoryStream())
            {
                _Serializer.Serialize(ms, this);

                ms.Seek(0, SeekOrigin.Begin);

                var data = new byte[ms.Length];
                ms.Read(data, 0, data.Length);

                return Convert.ToBase64String(data);
            }
        }
    }
}

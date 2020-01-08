using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace UMGS
{
    [System.Serializable]
    public class VirtualCurrency : INotifyPropertyChanged
    {
        private int _id;
        public string displayName;
        public bool IsAvailable => PlayerPrefs.HasKey(displayName);

        public int value
        {
            get => PlayerPrefs.GetInt(displayName);
            set
            {
                PlayerPrefs.SetInt(displayName, value);
                OnPropertyChanged();
            }
        }

        public int Id => _id;

        public VirtualCurrency(string displayName, int id)
        {
            this.displayName = displayName;
            _id = id;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}
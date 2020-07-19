using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BigClubDebate.Web.Annotations;

namespace BigClubDebate.Web.Pages.Components
{
    public class DatesFilter : INotifyPropertyChanged
    {
        DateTime? _est1;
        DateTime? _est2;

        public DatesFilter(Action loadGames)
        {
            PropertyChanged += (o,s) => loadGames();
        }

        public DatesFilter(Action loadGames, DateTime? team1CompetitionStart, DateTime? team2CompetitionStart)
        {
            _est1 = team1CompetitionStart;
            _est2 = team2CompetitionStart;
            PropertyChanged += (o, s) => loadGames();
        }

        public DateTime? Est1
        {
            get => _est1;
            set
            {
                _est1 = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Est2
        {
            get => _est2;
            set
            {
                _est2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
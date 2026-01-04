using Dots_and_boxes.ViewModels;
using Game.Model;
using Game.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots_and_boxes.Views
{
    public static class DesignData
    {
        public static GameViewModel ViewModel
        {
            get
            {
                var model = new GameModel(new BoxesFileDataAccess());
                model.NewGame(BoardSize.Small);
                // egy elindított játékot rakunk be a nézetmodellbe, így a tervezőfelületen sem csak üres cellák lesznek
                return new GameViewModel(model);
            }
        }
    }
}

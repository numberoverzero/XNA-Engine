using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Engine.Screen
{
    public interface IScreenComponent
    {
        string Name { get; set; }
        bool Selected { get; }
        bool Focused { get; }
        /// <summary>
        /// Toggle the Selected state
        /// </summary>
        void Select();
        /// <summary>
        /// Toggle the Focused state
        /// </summary>
        void Focus();

    }

    public abstract class GenericScreenComponent : IScreenComponent
    {
        protected GenericScreenComponent(string name)
        {
            Name = name;
            Selected = Focused = false;
        }

        #region IScreenComponent Members

        public string Name { get; set; }
        public bool Selected { get; private set; }
        public bool Focused { get; private set; }
        public virtual void Select()
        {
            Selected = !Selected;
        }

        public virtual void Focus()
        {
            Focused = !Focused;
        }

        #endregion
    }

    public class MenuItem : GenericScreenComponent
    {
        public Action OnSelect;
        public MenuScreen Screen;

        public MenuItem(string name, MenuScreen menuScreen, Action onSelect) : base(name)
        {
            Screen = menuScreen;
        }

        public UISystem UI
        {
            get { return Screen.UI; }
        }

        public override void Select()
        {
            if (!Selected && OnSelect != null) OnSelect();
            base.Select();
        }

        public MenuItem Copy()
        {
            return new MenuItem(Name, Screen, OnSelect);
        }
    }

    public abstract class Screen : GenericScreenComponent
    {
        public UISystem UI;

        protected Screen(string name, UISystem ui) : base(name)
        {
            UI = ui;
        }
        public void Exit()
        {
            
        }

        public abstract Screen Copy();
    }

    public class MenuScreen : Screen
    {
        private readonly List<MenuItem> _items;
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                var n = _items.Count;
                if (n == 0) value = 0;
                else if (value < 0 || value >= n) value %= n;
                _selectedIndex = value;
            }
        }

        public MenuScreen(string name, UISystem ui) : base(name, ui)
        {
            _items = new List<MenuItem>();
        }

        public void AddItem(MenuItem menuItem)
        {
            _items.Add(menuItem);
        }

        public void AddItems(IEnumerable<MenuItem> menuItems)
        {
            foreach(var menuItem in menuItems)
                AddItem(menuItem);
        }

        public override Screen Copy()
        {
            var menuScreen = new MenuScreen(Name, UI);
            menuScreen.AddItems(_items.Select(menuItem => menuItem.Copy()));
            return menuScreen;
        }

        /// <summary>
        /// Selects the currently focused (highlighted) item
        /// </summary>
        public override void Select()
        {
            _items[_selectedIndex].Select();
        }

        public static bool FromXml(UISystem ui, string filename, out MenuScreen menuScreen)
        {
            menuScreen = null;
            var file = new XmlDocument();
            file.Load(filename);
            var root = file["Menu"];
            if (root == null || root["Name"] == null || root["SelectedIndex"] == null) return false;

            var selectedIndex = root["SelectedIndex"].Value;

            var menuName = root["Name"].Value;
            menuScreen = new MenuScreen(menuName, ui);
            
            foreach (XmlNode itemNode in root.GetElementsByTagName("Item"))
            {
                if (itemNode["Text"] == null || itemNode["OnSelect"] == null) continue;
                var itemName = itemNode["Text"].Value;
                var onSelectNode = itemNode["OnSelect"];
                var onSelect = UISystem.ParseAction(onSelectNode);
                var menuItem = new MenuItem(itemName, menuScreen, onSelect);
                menuScreen.AddItem(menuItem);
            }
            
            // Assign index after items are added so we don't wrap the index accidentally.
            menuScreen.SelectedIndex = Int32.Parse(selectedIndex);

            
            return true;
        }
    }

    public class UISystem
    {
        private readonly List<Screen> _screenStack;
        private readonly Dictionary<string, Screen> _screens;

        public UISystem()
        {
            _screens = new Dictionary<string, Screen>();
            _screenStack = new List<Screen>();
        }

        /// <summary>
        ///   Open a specific menu.  If that menu isn't already loaded, loads an instance of the menu.
        ///   Otherwise, brings the loaded menu to the top of the stack
        /// </summary>
        public bool OpenMenu(string menuName)
        {
            if (!_screens.ContainsKey(menuName)) return false;
            var menu = _screens[menuName].Copy();
            _screenStack.Add(menu);
            return true;
        }

        /// <summary>
        /// Exits the top-most menu
        /// </summary>
        public void ExitMenu()
        {
            var index = _screenStack.Count - 1;
            if (index < 0) return;
            var screen = _screenStack[index];
            screen.Exit();
            _screenStack.RemoveAt(index);
        }

        public static Action ParseAction(XmlNode onSelectNode)
        {
            throw new NotImplementedException();
        }
    }
}
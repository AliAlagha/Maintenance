using Maintenance.Core.Constants;

namespace Maintenance.Core.Helpers
{
    public class SideMenuHelper
    {
        public string CurrentController { get; set; }
        public string CurrentAction { get; set; }

        public SideMenuHelper(string currentController, string currentAction)
        {
            CurrentController = currentController;
            CurrentAction = currentAction;
        }

        public string GetSubMenuClass(List<string> controllers)
        {
            if (controllers.Contains(CurrentController))
            {
                return SideMenuItems.OpenedSubmenuClass;
            }

            return SideMenuItems.ClosedSubmenuClass;
        }

        public string GetMenuItemClass(string controller, string action)
        {
            if (controller.Equals(CurrentController) && action.Equals(CurrentAction))
            {
                return SideMenuItems.ActiveItemClass;
            }

            return SideMenuItems.NotActiveItemClass;
        }

        public string GetProfileMenuItemClass(string controller, string action)
        {
            if (controller.Equals(CurrentController) && action.Equals(CurrentAction))
            {
                return SideMenuItems.ActiveProfileItemClass;
            }

            return SideMenuItems.NotActiveProfileItemClass;
        }

    }
}

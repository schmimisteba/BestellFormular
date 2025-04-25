using BestellFormular;
using System.Collections;

public class ScrollManager
{

    /// <summary>
    /// Scrolls the specified ScrollView to the top.
    /// </summary>
    /// <param name="contentPage">The ContentPage that contains the ScrollView.</param>
    /// <param name="scrollviewName">The name of the ScrollView to scroll.</param>
    public static void ScrollViewToTop(ContentPage contentPage, string scrollviewName = "ProductScrollView")
    {
        var scrollView = contentPage?.FindByName<ScrollView>(scrollviewName);
        if (scrollView != null)
        {
            scrollView.ScrollToAsync(0, 0, true);  // Scroll smoothly to the top
        }
    }

    /// <summary>
    /// Scrolls the specified ScrollView to the top.
    /// </summary>
    /// <param name="contentPage">The ContentPage that contains the ScrollView.</param>
    /// <param name="scrollviewName">The name of the ScrollView to scroll.</param>
    [Obsolete]
    public static void ScrollViewToBottom(ContentPage contentPage, string scrollviewName = "ProductScrollView")
    {
        var scrollView = contentPage?.FindByName<ScrollView>(scrollviewName);
        if (scrollView != null)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(50);
                scrollView.ScrollToAsync(0, double.MaxValue, true);  // Scroll smoothly to the top
            });
        }
    }

    /// <summary>
    /// Scrolls the specified ScrollView to the top.
    /// </summary>
    /// <param name="contentPage">The ContentPage that contains the ScrollView.</param>
    /// <param name="scrollviewName">The name of the ScrollView to scroll.</param>
    [Obsolete]
    public static void ScrollViewToY(ContentPage contentPage, double y, string scrollviewName = "ProductScrollView")
    {
        var scrollView = contentPage?.FindByName<ScrollView>(scrollviewName);
        if (scrollView != null)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(50);
                scrollView.ScrollToAsync(0, y, true);  // Scroll smoothly to the top
            });
        }
    }

    /// <summary>
    /// Scrolls the specified ListView to the last selected item.
    /// </summary>
    /// <param name="listviewName">The name of the ListView to scroll.</param>
    public static void ListViewToSelected(string listviewName = "ProductsListView")
    {
        // Get the ListView from the current page
        var listView = (App.Current.MainPage as ContentPage)?.FindByName<ListView>(listviewName);

        // Check if the ListView has items
        if (listView?.ItemsSource is IEnumerable items && items.Cast<object>().Any())
        {
            var lastItem = items.Cast<object>().LastOrDefault(); // Get the last item
            if (lastItem != null)
            {
                listView.ScrollTo(lastItem, ScrollToPosition.End, true);  // Scroll smoothly to the last item
            }
        }
    }
}

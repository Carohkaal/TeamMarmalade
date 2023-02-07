using System;
using System.Reflection;
using Turnip.Models;

namespace Turnip.Services
{
    internal static class ResourceAccessor
    {
        public static Uri Get(string resourcePath)
        {
            var uri = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetExecutingAssembly().GetName().Name
                , resourcePath
            );

            return new Uri(uri);
        }
    }

    public interface INavigationService
    {
        void NavigateTo(string pageName);

        event EventHandler? Navigate;
    }

    public class PageNavigationService : INavigationService
    {
        public void NavigateTo(string pageName)
        {
            if (Navigate == null) return;

            Navigate.Invoke(this, new PageNavigationEventArgs(pageName));
        }

        public event EventHandler? Navigate;
    }
}
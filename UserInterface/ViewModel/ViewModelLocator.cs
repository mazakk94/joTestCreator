using Wojtasik.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Wojtasik.DataAccessObject.DataObjects;

namespace Wojtasik.UserInterface.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            else
                SimpleIoc.Default.Register<IDataService, DataService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SolveTestViewModel>();
            SimpleIoc.Default.Register<CreateTestViewModel>();
            SimpleIoc.Default.Register<CreateQuestionViewModel>(); 
            SimpleIoc.Default.Register<HistoryViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
        
        public static void Cleanup() { }
    }
}
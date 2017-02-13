namespace ReactiveUI.XamForms
{
    internal static class BindingContextHelper
    {
        public static void AssignViewModelFromBindingContext<TViewModel>(this IViewFor<TViewModel> This, object bindingContext)
            where TViewModel : class
        {
            // only assign the VM if the binding context is explicitly null, or is an object of type TViewModel. This allows for
            // the ViewModel property to be set to one object, whilst the BindingContext can be set to something else.

            if (bindingContext == null) {
                This.ViewModel = null;
            } else {
                var viewModel = bindingContext as TViewModel;

                if (viewModel != null) {
                    This.ViewModel = viewModel;
                }
            }
        }
    }
}
namespace LoginPage.Models
{
    /********************************************************************************************************************************************
     This file defines the ErrorViewModel class, which represents error information in the application.
     Key components and their purposes:
     - Define the ErrorViewModel class with properties to store error information.
     - Properties:
       - RequestId: Stores the unique identifier for the request that caused the error.
       - ShowRequestId: Computed property that indicates whether the RequestId should be shown (i.e., if it is not null or empty).
    ***********************************************************************************/
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

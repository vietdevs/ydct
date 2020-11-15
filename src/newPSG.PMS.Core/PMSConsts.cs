namespace newPSG.PMS
{
    /// <summary>
    /// Some general constants for the application.
    /// </summary>
    public class PMSConsts
    {
        public const string LocalizationSourceName = "PMS";

        public const bool MultiTenancyEnabled = true;
    }

    public class ExcuteZeroResult
    {
        public bool IsError { set; get; }

        private object _Value;
        public object Value
        {
            get
            {
                return _Value;
            }
            set { _Value = value; }
        }

        public string Message { set; get; }

        public string Selected { set; get; }

        public bool IsValid { set; get; }

        public ExcuteZeroResult()
        {
            IsError = false;
            IsValid = true;
        }
    }
}
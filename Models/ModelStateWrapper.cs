namespace AttestationProject.Models
{
    // During the initialization of services we create and pass the "ModelStateWrapper" object to decouple Controler and Service layers coupled by the ModelState
    public class ModelStateWrapper : Interfaces.IValidationDictionary
    {
        #region Initialization

        private ModelStateDictionary _modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        #endregion


        #region IValidationDictionary Members

        public void AddError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            get { return _modelState.IsValid; }
        }

        public string Errors()
        {
            // Creating the error message to be returned in case of ModelState violations
            StringBuilder errors = new StringBuilder();
            foreach (KeyValuePair<string, ModelStateEntry> kvp in _modelState)
            {
                foreach (var error in kvp.Value.Errors)
                {
                    errors.Append("\n" + error.ErrorMessage);
                }
            }
            return errors.ToString().Trim();
        }

        #endregion
    }
}

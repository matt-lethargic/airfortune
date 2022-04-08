using AirFortune.Airtable;
using AirFortune.Airtable.Exceptions;
using AirFortune.Airtable.Responses;

namespace AirFortune.Services
{
    public class AirtableValidationResponse
    {
        public bool Valid => Errors.Count == 0;
        public Dictionary<string, string> Errors { get; }

        public AirtableValidationResponse()
        {
            Errors = new Dictionary<string, string>();
        }
    }

    public class AirtableService
    {
        public async Task<AirtableValidationResponse> ValidateTable(string apiKey, string baseId, string tableName, 
            string firstNameField, string lastNameField)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            if (baseId == null) throw new ArgumentNullException(nameof(baseId));

            string? offset = null;
            string? errorMessage = null;

            AirtableValidationResponse validationResponse = new AirtableValidationResponse();

            using (AirtableBase airtableBase = new AirtableBase(apiKey, baseId))
            {
                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName, offset, pageSize: 1);

                AirtableListRecordsResponse response = await task;
                
                if (response.Success)
                {
                    if (response.Records == null)
                    {
                        validationResponse.Errors.Add("Name", "No records in table");
                        return validationResponse;
                    }

                    if (!response.Records.Any(x => x.Fields.ContainsKey(firstNameField)))
                    {
                        validationResponse.Errors.Add("FirstNameField", "FirstNameField not found");
                    }


                    if (!response.Records.Any(x => x.Fields.ContainsKey(lastNameField)))
                    {
                        validationResponse.Errors.Add("LastNameField", "LastNameField not found");
                    }
                }
                else if (response.AirtableApiError is AirtableNotFoundException)
                {
                    validationResponse.Errors.Add("Name", "Table not found");
                }
                else if (response.AirtableApiError != null)
                {
                    errorMessage = response.AirtableApiError.ErrorMessage;
                }
                else
                {
                    errorMessage = "Unknown error";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return validationResponse;
        }

        public async Task<IEnumerable<string>?> GetNamesAsync(string apiKey, string baseId, string tableName, 
            string firstNameField, string lastNameField)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            if (baseId == null) throw new ArgumentNullException(nameof(baseId));

            string? offset = null;
            string? errorMessage = null;
            List<string> names = new List<string>();

            using (AirtableBase airtableBase = new AirtableBase(apiKey, baseId))
            {
                do
                {
                    Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName, offset);

                    AirtableListRecordsResponse response = await task;

                    if (response.Success)
                    {
                        if (response.Records != null)
                        {
                            names.AddRange(response.Records.Select(x => $" {x.Fields[firstNameField]} {x.Fields[lastNameField]}"));
                        }

                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError != null)
                    {
                        errorMessage = response.AirtableApiError.ErrorMessage;
                        break;
                    }
                    else
                    {
                        errorMessage = "Unknown error";
                        break;
                    }

                } while (offset != null);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return names;
        }
    }
}

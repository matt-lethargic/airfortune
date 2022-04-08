using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using AirFortune.Airtable.Exceptions;
using AirFortune.Airtable.Responses;

namespace AirFortune.Airtable;

public class AirtableBase : IDisposable
{
    private enum OperationType
    {
        Create, 
        Update, 
        Replace
    };

    private const int MaxPageSize = 100;

    private const string AirtableApiUrl = "https://api.airtable.com/v0/";

    private readonly string _baseId;
    private readonly HttpClientWithRetries _httpClientWithRetries;
    private readonly JsonSerializerOptions _jsonOptionIgnoreNullValues = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public bool ShouldNotRetryIfRateLimited
    {
        get => _httpClientWithRetries.ShouldNotRetryIfRateLimited;
        set => _httpClientWithRetries.ShouldNotRetryIfRateLimited = value;
    }

    public int RetryDelayMillisecondsIfRateLimited
    {
        get => _httpClientWithRetries.RetryDelayMillisecondsIfRateLimited;
        set => _httpClientWithRetries.RetryDelayMillisecondsIfRateLimited = value;
    }


    public AirtableBase(string apiKey, string baseId) 
        : this(apiKey, baseId, null)
    {
    }

    internal AirtableBase(string apiKey, string baseId, DelegatingHandler? delegatingHandler)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("apiKey cannot be null", nameof(apiKey));
        }

        if (string.IsNullOrEmpty(baseId))
        {
            throw new ArgumentException("baseId cannot be null", nameof(baseId));
        }

        _baseId = baseId;
        _httpClientWithRetries = new HttpClientWithRetries(delegatingHandler, apiKey);
    }


    /// <summary>
    /// Called to get a list of records in the table specified by 'tableName'
    /// </summary>
    public async Task<AirtableListRecordsResponse> ListRecords(
        string tableName,
        string? offset = null,
        IEnumerable<string>? fields = null,
        string? filterByFormula = null,
        int? maxRecords = null,
        int? pageSize = null,
        IEnumerable<Sort>? sort = null,
        string? view = null)
    {
        HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
            maxRecords, pageSize, sort, view).ConfigureAwait(false);

        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableListRecordsResponse(error);
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        AirtableRecordList? recordList = JsonSerializer.Deserialize<AirtableRecordList>(responseBody, _jsonOptionIgnoreNullValues);
        return new AirtableListRecordsResponse(recordList);
    }


    /// <summary>
    /// Called to get a list of <see cref="AirtableListRecordsResponse{T}"/> in the table specified by 'tableName'
    /// The fields of each record are deserialized to type {T}.
    /// </summary>
    public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
        string tableName,
        string? offset = null,
        IEnumerable<string>? fields = null,
        string? filterByFormula = null,
        int? maxRecords = null,
        int? pageSize = null,
        IEnumerable<Sort>? sort = null,
        string? view = null)
    {
        HttpResponseMessage response = await ListRecordsInternal(tableName, offset, fields, filterByFormula,
            maxRecords, pageSize, sort, view).ConfigureAwait(false);
        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableListRecordsResponse<T>(error);
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        AirtableRecordList<T>? recordList = JsonSerializer.Deserialize<AirtableRecordList<T>>(responseBody);
        return new AirtableListRecordsResponse<T>(recordList);
    }

    

    /// <summary>
    /// Called to retrieve a record with the specified id from the specified table.
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AirtableRetrieveRecordResponse> RetrieveRecord(string tableName, string id)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table Name cannot be null", nameof(tableName));
        }

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Record ID cannot be null", nameof(id));
        }

        string uriStr = AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
        var request = new HttpRequestMessage(HttpMethod.Get, uriStr);
        var response = await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableRetrieveRecordResponse(error);
        }
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var airtableRecord = JsonSerializer.Deserialize<AirtableRecord>(responseBody, _jsonOptionIgnoreNullValues);

        return new AirtableRetrieveRecordResponse(airtableRecord);
    }

    
    /// <summary>
    /// Called to retrieve a record with the specified id from the specified table.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AirtableRetrieveRecordResponse<T>> RetrieveRecord<T>(string tableName, string id)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table Name cannot be null", nameof(tableName));
        }

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Record ID cannot be null", nameof(id));
        }

        string uriStr = AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
        var request = new HttpRequestMessage(HttpMethod.Get, uriStr);
        var response = await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableRetrieveRecordResponse<T>(error);
        }
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        AirtableRecord<T>? airtableRecord = JsonSerializer.Deserialize<AirtableRecord<T>>(responseBody);

        return new AirtableRetrieveRecordResponse<T>(airtableRecord);
    }
    
    /// <summary>
    /// Called to create a record in the specified table.
    /// </summary>
    public async Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(
        string tableName,
        Fields fields,
        bool typecast = false)
    {
        Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.Create, typecast: typecast);
        var response = await task.ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// Called to update a record with the specified ID in the specified table.
    /// </summary>
    public async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(
        string tableName,
        Fields fields,
        string id,
        bool typeCast = false)
    {
        Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.Update, id, typeCast);
        var response = await task.ConfigureAwait(false);
        return response;
    }

    
    /// <summary>
    /// Called to replace a record with the specified ID in the specified table using jsoncnotent.
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="fields"></param>
    /// <param name="id"></param>
    /// <param name="typeCast"></param>
    /// <returns></returns>
    public async Task<AirtableCreateUpdateReplaceRecordResponse> ReplaceRecord(
        string tableName,
        Fields fields,
        string id,
        bool typeCast = false)
    {
        Task<AirtableCreateUpdateReplaceRecordResponse> task = CreateUpdateReplaceRecord(tableName, fields, OperationType.Replace, id, typeCast);
        return await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Called to delete a record with the specified ID in the specified table
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AirtableDeleteRecordResponse> DeleteRecord(
        string tableName,
        string id)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table name cannot be null", nameof(tableName));
        }

        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Record ID cannot be null", nameof(id));
        }

        string uriStr = AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName) + "/" + id;
        var request = new HttpRequestMessage(HttpMethod.Delete, uriStr);
        var response = await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableDeleteRecordResponse(error);
        }
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var deletedRecord = JsonSerializer.Deserialize<AirtableDeletedRecord>(responseBody);

        if (deletedRecord == null)
            return new AirtableDeleteRecordResponse(false, null);
        
        return new AirtableDeleteRecordResponse(deletedRecord.Deleted, deletedRecord.Id);
    }

    /// <summary>
    /// Called to create multiple records in the specified table in one single operation.
    /// </summary>
    /// <returns></returns>
    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateMultipleRecords(
        string tableName,
        Fields[] fields,
        bool typecast = false)
    {
        var json = JsonSerializer.Serialize(new { records = fields, typecast }, _jsonOptionIgnoreNullValues);
        Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Post, json);
        var response = await task.ConfigureAwait(false);
        return response;

    }

    /// <summary>
    /// Called to update multiple records with the specified IDs in the specified table in one single operation.
    /// </summary>
    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> UpdateMultipleRecords(
        string tableName,
        IdFields[] idFields,
        bool typecast = false)
    {
        var json = JsonSerializer.Serialize(new { records = idFields, typecast }, _jsonOptionIgnoreNullValues);
        Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, new HttpMethod("PATCH"), json);
        return await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Called to update multiple records with the specified IDs in the specified table in one single operation.
    /// </summary>
    public async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> ReplaceMultipleRecords(
        string tableName,
        IdFields[] idFields,
        bool typecast = false)
    {
        var json = JsonSerializer.Serialize(new { records = idFields, typecast }, _jsonOptionIgnoreNullValues);
        Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task = CreateUpdateReplaceMultipleRecords(tableName, HttpMethod.Put, json);
        return await task.ConfigureAwait(false);
    }
    
   
    /// <summary>
    /// Build URI for the List Records operation
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private Uri BuildUriForListRecords(
        string tableName, string? offset, IEnumerable<string>? fields,
        string? filterByFormula, int? maxRecords,
        int? pageSize, IEnumerable<Sort>? sort, string? view)
    {
        var uriBuilder = new UriBuilder(AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName));

        if (!string.IsNullOrEmpty(offset))
        {
            AddParametersToQuery(ref uriBuilder, $"offset={HttpUtility.UrlEncode(offset)}");
        }

        if (fields != null)
        {
            string flattenFieldsParam = ParameterFlattener.FlattenFieldsParam(fields);
            AddParametersToQuery(ref uriBuilder, flattenFieldsParam);
        }

        if (!string.IsNullOrEmpty(filterByFormula))
        {
            AddParametersToQuery(ref uriBuilder, $"filterByFormula={HttpUtility.UrlEncode(filterByFormula)}");
        }

        if (sort != null)
        {
            string flattenSortParam = ParameterFlattener.FlattenSortParam(sort);
            AddParametersToQuery(ref uriBuilder, flattenSortParam);
        }

        if (!string.IsNullOrEmpty(view))
        {
            AddParametersToQuery(ref uriBuilder, $"view={HttpUtility.UrlEncode(view)}");
        }

        if (maxRecords != null)
        {
            if (maxRecords <= 0)
            {
                throw new ArgumentException("Maximum Number of Records must be > 0", nameof(maxRecords));
            }
            AddParametersToQuery(ref uriBuilder, $"maxRecords={maxRecords}");
        }

        if (pageSize != null)
        {
            if (pageSize is <= 0 or > MaxPageSize)
            {
                throw new ArgumentException("Page Size must be > 0 and <= 100", nameof(pageSize));
            }
            AddParametersToQuery(ref uriBuilder, $"pageSize={pageSize}");
        }
        return uriBuilder.Uri;
    }
    
    /// <summary>
    /// Helper function for URI parameters
    /// </summary>
    /// <param name="uriBuilder"></param>
    /// <param name="queryToAppend"></param>
    private void AddParametersToQuery(ref UriBuilder uriBuilder, string queryToAppend)
    {
        if (uriBuilder.Query.Length > 1)
        {
            uriBuilder.Query = uriBuilder.Query[1..] + "&" + queryToAppend;
        }
        else
        {
            uriBuilder.Query = queryToAppend;
        }
    }


    /// <summary>
    /// Worker function which does the real work for creating, updating, or replacing a record
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private async Task<AirtableCreateUpdateReplaceRecordResponse> CreateUpdateReplaceRecord(
        string tableName,
        Fields fields,
        OperationType operationType,
        string? recordId = null,
        bool typecast = false)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table Name cannot be null", nameof(tableName));
        }

        string uriStr = AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName) + "/";
        HttpMethod httpMethod;
        switch (operationType)
        {
            case OperationType.Update:
                uriStr += recordId + "/";
                httpMethod = new HttpMethod("PATCH");
                break;
            case OperationType.Replace:
                uriStr += recordId + "/";
                httpMethod = HttpMethod.Put;
                break;
            case OperationType.Create:
                httpMethod = HttpMethod.Post;
                break;
            default:
                throw new ArgumentException("Operation Type must be one of { OperationType.UPDATE, OperationType.REPLACE, OperationType.CREATE }", nameof(operationType));
        }

        var fieldsAndTypecast = new { fields = fields.FieldsCollection, typecast };
        var json = JsonSerializer.Serialize(fieldsAndTypecast, _jsonOptionIgnoreNullValues);
        var request = new HttpRequestMessage(httpMethod, uriStr);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableCreateUpdateReplaceRecordResponse(error);
        }
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var airtableRecord = JsonSerializer.Deserialize<AirtableRecord>(responseBody);

        return new AirtableCreateUpdateReplaceRecordResponse(airtableRecord);
    }

    
    /// <summary>
    /// Worker function which does the real work for creating, updating or replacing multiple records
    /// in one operation
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private async Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> CreateUpdateReplaceMultipleRecords(string tableName, HttpMethod method, string json)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table Name cannot be null", nameof(tableName));
        }
        string uriStr = AirtableApiUrl + _baseId + "/" + Uri.EscapeDataString(tableName) + "/";
        var request = new HttpRequestMessage(method, uriStr);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);

        AirtableApiException? error = await CheckForAirtableException(response).ConfigureAwait(false);
        if (error != null)
        {
            return new AirtableCreateUpdateReplaceMultipleRecordsResponse(error);
        }
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var recordList = JsonSerializer.Deserialize<AirtableRecordList>(responseBody);

        return new AirtableCreateUpdateReplaceMultipleRecordsResponse(recordList?.Records);
    }
    
    /// <summary>
    /// Construct and return the appropriate exception based on the specified message response
    /// </summary>
    /// <exception cref="AirtableUnrecognizedException"></exception>
    private async Task<AirtableApiException?> CheckForAirtableException(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                return null;

            case System.Net.HttpStatusCode.BadRequest:
                return new AirtableBadRequestException();

            case System.Net.HttpStatusCode.Forbidden:
                return new AirtableForbiddenException();

            case System.Net.HttpStatusCode.NotFound:
                return new AirtableNotFoundException();

            case System.Net.HttpStatusCode.PaymentRequired:
                return new AirtablePaymentRequiredException();

            case System.Net.HttpStatusCode.Unauthorized:
                return new AirtableUnauthorizedException();

            case System.Net.HttpStatusCode.RequestEntityTooLarge:
                return new AirtableRequestEntityTooLargeException();

            case (System.Net.HttpStatusCode)422:    // There is no HttpStatusCode.InvalidRequest defined in HttpStatusCode Enumeration.
                var error = await ReadResponseErrorMessage(response).ConfigureAwait(false);
                return new AirtableInvalidRequestException(error);

            case System.Net.HttpStatusCode.TooManyRequests:
                return new AirtableTooManyRequestsException();

            default:
                throw new AirtableUnrecognizedException(response.StatusCode);
        }
    }

    /// <summary>
    /// Attempts to read the error message in the response body.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task<string?> ReadResponseErrorMessage(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        MessagePart? json = JsonSerializer.Deserialize<MessagePart>(content);
        if (json?.Error != null)
        {
            return json.Error;
        }
        return json?.Message;
    }
    
    private async Task<HttpResponseMessage> ListRecordsInternal(
        string tableName,
        string? offset,
        IEnumerable<string>? fields,
        string? filterByFormula,
        int? maxRecords,
        int? pageSize,
        IEnumerable<Sort>? sort,
        string? view)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table Name cannot be null", nameof(tableName));
        }
        var uri = BuildUriForListRecords(tableName, offset, fields, filterByFormula, maxRecords, pageSize, sort, view);
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await _httpClientWithRetries.SendAsync(request).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _httpClientWithRetries.Dispose();
    }


    class MessagePart
    {
        public string? Error { get; set; }
        public string? Message { get; set; }
    }
}
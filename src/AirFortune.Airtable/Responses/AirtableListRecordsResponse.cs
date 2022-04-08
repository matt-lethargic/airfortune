
using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;
public class AirtableListRecordsResponse<T> : AirtableApiResponse
{
    public AirtableListRecordsResponse(AirtableApiException? error) : base(error)
    {
        Offset = null;
        Records = null;
    }
    
    public AirtableListRecordsResponse(AirtableRecordList<T>? recordList)
    {
        Offset = recordList?.Offset;
        Records = recordList?.Records;
    }

    public IEnumerable<AirtableRecord>? Records { get; }
    public string? Offset { get; }
}

public class AirtableListRecordsResponse : AirtableApiResponse
{
    public AirtableListRecordsResponse(AirtableApiException error) 
        : base(error) { }

    public AirtableListRecordsResponse(AirtableRecordList? recordList)
    {
        Offset = recordList.Offset;
        Records = recordList.Records;
    }

    public IEnumerable<AirtableRecord>? Records { get; }
    public string? Offset { get; }
}
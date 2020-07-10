using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

//template only. feel free to edit
namespace Abc.HabitTracker.Api
{
  public class RequestData
  {
    [JsonPropertyName("name")]
    public String Name { get; set; }

    [JsonPropertyName("days_off")]
    public String[] DaysOff { get; set; }
  }

  public class HabitAPI
  {
    [JsonPropertyName("id")]
    public Guid ID { get; set; }

    [JsonPropertyName("name")]
    public String Name { get; set; }

    [JsonPropertyName("days_off")]
    public String[] DaysOff { get; set; }

    [JsonPropertyName("current_streak")]
    public Int16 CurrentStreak { get; set; }

    [JsonPropertyName("longest_streak")]
    public Int16 LongestStreak { get; set; }

    [JsonPropertyName("log_count")]
    public Int16 LogCount { get; set; }

    [JsonPropertyName("logs")]
    public IEnumerable<DateTime> Logs { get; set; }

    [JsonPropertyName("user_id")]
    public Guid UserID { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
  }
}

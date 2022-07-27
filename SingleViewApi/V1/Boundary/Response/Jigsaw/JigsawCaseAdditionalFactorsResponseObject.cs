using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServiceStack;

namespace SingleViewApi.V1.Boundary.Response;

public class JigsawCaseAdditionalFactorsResponseObject
{

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("header")]
    public string Header { get; set; }
    [JsonProperty("footer")]
    public string Footer { get; set; }
    [JsonProperty("formTitle")]
    public string FormTitle { get; set; }
    [JsonProperty("isBuddyForm")]
    public bool IsBuddyForm { get; set; }
    [JsonProperty("questionGroups")]
    public List<QuestionGroup> QuestionGroups { get; set; }
    [JsonProperty("mapCode")]
    public string MapCode { get; set; }
}
public class Option
{
    [JsonProperty("label")]
    public string Label { get; set; }
    [JsonProperty("value")]
    public string Value { get; set; }
}

public class Question
{
    [JsonProperty("selectedValue")]
    public string SelectedValue { get; set; }
    [JsonProperty("option")]
    public List<Option> Options { get; set; }
    [JsonProperty("triggerQuestions")]
    public List<object> TriggerQuestions { get; set; }
    [JsonProperty("questionType")]
    public int QuestionType { get; set; }
    [JsonProperty("label")]
    public string Label { get; set; }
    [JsonProperty("questionMapCode")]
    public string QuestionMapCode { get; set; }
    [JsonProperty("startNewLine")]
    public bool StartNewLine { get; set; }
    [JsonProperty("mandatory")]
    public bool Mandatory { get; set; }
    [JsonProperty("readOnly")]
    public bool ReadOnly { get; set; }
    [JsonProperty("maxLength")]
    public int? MaxLength { get; set; }
    [JsonProperty("rows")]
    public int? Rows { get; set; }

    public string GetAnswer(string value)
    {
        return Options.IsEmpty() ? value : Options.Find(o => o.Value == value)?.Label;
    }
}

public class QuestionGroup
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("legend")]
    public string Legend { get; set; }
    [JsonProperty("questions")]
    public List<Question> Questions { get; set; }
}



namespace Tracker.Client.Dtos
{
    public class GitHubProjectV2ResponseDto
    {
        public Data? data { get; set; }
    }

    public class Data
    {
        public Organization? organization { get; set; }
    }

    public class Organization
    {
        public ProjectV2? projectV2 { get; set; }
    }

    public class ProjectV2
    {
        public string? id { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public ViewConnection? views { get; set; }
        public ItemConnection? items { get; set; }
    }

    public class ViewConnection
    {
        public List<ProjectView>? nodes { get; set; } = new();
    }

    public class ProjectView
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public int? number { get; set; }
        public string? layout { get; set; }
    }

    public class ItemConnection
    {
        public List<ProjectItem>? nodes { get; set; } = new();
        public PageInfo? pageInfo { get; set; } = new();
    }

    public class PageInfo
    {
        public bool? hasNextPage { get; set; }
        public string? endCursor { get; set; }
    }

    public class ProjectItem
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public Content? content { get; set; }
        public FieldValueConnection? fieldValues { get; set; }
    }

    public class Content
    {
        public string? title { get; set; }
        public string? url { get; set; }
        public int? number { get; set; }
        public string? state { get; set; }
        public bool? merged { get; set; }
        public AssigneeConnection? assignees { get; set; }
    }

    public class AssigneeConnection
    {
        public List<Assignee>? nodes { get; set; } = new();
    }

    public class Assignee
    {
        public string? login { get; set; }
        public string? name { get; set; }
        public string? avatarUrl { get; set; }
    }

    public class FieldValueConnection
    {
        public List<FieldValue>? nodes { get; set; } = new();
    }

    public class FieldValue
    {
        public string? __typename { get; set; }
        public Field? field { get; set; }

        // For TextValue
        public string? text { get; set; }

        // For SingleSelectValue
        public string? name { get; set; }

        // For NumberValue
        public double? number { get; set; }

        // For DateValue
        public string? date { get; set; }

        // For UserValue (Assignees from custom field)
        public UserConnection? users { get; set; }
    }

    public class UserConnection
    {
        public List<User>? nodes { get; set; } = new();
    }

    public class User
    {
        public string? login { get; set; }
        public string? name { get; set; }
        public string? avatarUrl { get; set; }
    }

    public class Field
    {
        public string? __typename { get; set; }
        public string? name { get; set; }
    }

}
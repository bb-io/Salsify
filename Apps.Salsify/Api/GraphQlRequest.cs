using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Salsify.Api;

public class GraphQlRequest : RestRequest
{
    private const string Endpoint = "graphql";
    public const string OrganizationVariable = "organizationId";

    public GraphQlRequest(string operationName, string query, object variables) : this(operationName, query, JObject.FromObject(variables)) { }

    private GraphQlRequest(string operationName, string query, JObject variables) : base(Endpoint, Method.Post)
    {
        OperationName = operationName;
        Query = query;
        Variables = variables;

        this.AddStringBody(JsonConvert.SerializeObject(new { operationName, variables, query }), ContentType.Json);
    }

    public string OperationName { get; }
    public string Query { get; }
    public JObject Variables { get; }

    public bool DeclaresVariable(string name) => Query.Contains($"${name}");

    internal GraphQlRequest WithVariable(string name, JToken value)
    {
        var variables = (JObject)Variables.DeepClone();
        variables[name] = value;

        return new GraphQlRequest(OperationName, Query, variables);
    }
    
    internal GraphQlRequest ForPage(int page, int size) => WithVariable("pagination", JObject.FromObject(new { page, size }));
}
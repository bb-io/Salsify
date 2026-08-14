namespace Apps.Salsify.Constants.GraphQl;

public static class GraphQlMutations
{
    public const string UpdateEnumeratedValue =
        """
        mutation UpdateEnumeratedValue($input: UpdateEnumeratedValueInput!, $contentLocaleIds: [LocaleCode!]) {
          updateEnumeratedValue(input: $input) {
            enumeratedValue {
              id
              externalId
              name
              names(contentLocaleCodes: $contentLocaleIds) {
                locale { code }
                value
              }
            }
          }
        }
        """;
}
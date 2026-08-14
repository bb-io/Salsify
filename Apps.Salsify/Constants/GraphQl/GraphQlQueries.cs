namespace Apps.Salsify.Constants.GraphQl;

public static class GraphQlQueries
{
    public const string PropertyIndex =
        """
        query PropertyIndex($organizationId: ID!, $query: String, $pagination: PaginationInput!,
                            $orderBy: [PropertyOrderInput!], $dataType: [DataType!], $localizable: PropertyLocalizable) {
          organization(id: $organizationId) {
            properties(query: $query, pagination: $pagination, orderBy: $orderBy,
                       dataType: $dataType, localizable: $localizable) {
              pageMetadata { totalEntries }
              entries { id externalId name dataType localizable propertyGroup { externalId name } }
            }
          }
        }
        """;
    
    public const string EnumeratedValues =
        """
        query EnumeratedValues($organizationId: ID!, $propertyId: Identifier!, $pagination: PaginationInput!,
                               $flatten: Boolean, $query: String, $contentLocalesCodes: [LocaleCode!]) {
          organization(id: $organizationId) {
            property(id: $propertyId) {
              enumeratedValues(flatten: $flatten, pagination: $pagination, query: $query) {
                entries {
                  id
                  externalId
                  name
                  names(contentLocaleCodes: $contentLocalesCodes) {
                    value
                    locale {
                      code
                    }
                  }
                }
                pageMetadata {
                  totalEntries
                  hasNext
                }
              }
            }
          }
        }
        """;
}
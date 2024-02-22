namespace Backend.Api.RequestModels;

public record CreateListRequestBodyModel(string Name, ICollection<string> PlayersIds);

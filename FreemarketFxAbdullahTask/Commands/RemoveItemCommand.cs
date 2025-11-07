namespace FreemarketFxAbdullahTask.Commands;

public class RemoveItemCommand : ICommand<bool>
{
    public Guid BasketId { get; set; }
    public Guid ItemId { get; set; }
}


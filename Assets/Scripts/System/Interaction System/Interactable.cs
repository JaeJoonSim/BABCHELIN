public interface Interactable
{
    public string InteractionPrompt { get; }
    public bool OnInteract(Interactor interactor);
    public void OffInteract();
}

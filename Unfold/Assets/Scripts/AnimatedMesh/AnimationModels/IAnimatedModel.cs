
namespace AnimatedMesh.AnimationModels
{
    public interface IAnimatedModel
    {
        bool IsSet { get; }
        void Clear();
        void UpdateModel();
    }
}

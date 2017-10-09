using Unfold;

namespace AnimatedMesh.AnimationModels
{
    public interface IAnimationModel
    {
        bool NeedToGenerateChildren(TriangleVertices vertices);
    }
}

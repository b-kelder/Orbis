using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     Interface for basicElements that can be updated.
    /// </summary>
    public interface IUpdatableElement : IBasicElement
    {
        /// <summary>
        ///     Perform this frame's update.
        /// </summary>
        void Update();
    }
}

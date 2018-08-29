using System;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// A header or footer of a table section.
    /// </summary>
    public class TableSectionHeader
    {
        /// <summary>
        /// Gets the function that creates the <see cref="UIView"/>
        /// used as header for this section. Overrides Title.
        /// </summary>
        public Func<UIView> View { get; protected set; }

        /// <summary>
        /// Gets the height of the header.
        /// </summary>
        public float Height { get; protected set; }

        /// <summary>
        /// Gets the title for the section header, only used if View is null.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSectionHeader"/>
        /// struct.
        /// </summary>
        /// <param name="view">Function that creates header's <see cref="UIView"/>.</param>
        /// <param name="height">Height of the header.</param>
        public TableSectionHeader(Func<UIView> view, float height)
        {
            View = view;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSectionHeader"/> class.
        /// </summary>
        /// <param name="title">Title to use.</param>
        public TableSectionHeader(string title)
        {
            Title = title;
        }
    }
}
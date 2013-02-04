// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Disposable.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   <para>Implements design pattern for the IDisposable interface.</para>
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;

    /// <summary>
    ///     <para>Implements design pattern for the IDisposable interface.</para>
    /// </summary>
    /// <remarks>
    ///     Derived class should override protected Dispose method (with bool parameter).
    ///     To do so use the following pattern.
    /// protected override void Dispose(bool disposing) 
    /// {
    ///     if (disposing) 
    ///     {
    ///         // Release managed resources.
    ///     }
    ///     // Release unmanaged resources.
    ///     // Set large fields to null.
    ///     // Call Dispose on your base class.
    ///     base.Dispose(disposing);
    ///  }
    /// </remarks>
    public abstract class Disposable : IDisposable
    {
        #region Fields

        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class. 
        ///     <para>The <see cref="Disposable"/> class finalizer.</para>
        /// </summary>
        ~Disposable()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     <para>The <see cref="System.Boolean"/> which indicates whether the object has been disposed.</para>
        /// </summary>
        protected bool Disposed
        {
            get
            {
                return this.disposed;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     <para>Releases all resources used by the parent class.</para>
        /// </summary>
        public void Dispose()
        {
            // Allow a Dispose method to be called more than once without throwing an exception.
            // The method should do nothing after the first call.
            if (!this.Disposed)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        /// <summary><para>Releases the unmanaged resources used by the parent class and optionally releases the managed resources.</para>
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.disposed = true;
        }

        /// <summary>
        ///     <para>Checks whether the <see cref="Disposable"/> has been disposed.</para>
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        ///     <para>The object has been disposed.</para>
        /// </exception>
        protected void ThrowErrorIfDisposed()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
        }

        #endregion
    }
}
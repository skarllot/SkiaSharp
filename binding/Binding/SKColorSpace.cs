﻿using System;
using System.ComponentModel;

namespace SkiaSharp
{
	public unsafe class SKColorSpace : SKObject, ISKReferenceCounted
	{
		private static readonly SKColorSpace srgb;
		private static readonly SKColorSpace srgbLinear;

		static SKColorSpace ()
		{
			srgb = new SKColorSpaceStatic (SkiaApi.sk_colorspace_new_srgb ());
			srgbLinear = new SKColorSpaceStatic (SkiaApi.sk_colorspace_new_srgb_linear ());
		}

		internal static void EnsureStaticInstanceAreInitialized ()
		{
			// IMPORTANT: do not remove to ensure that the static instances
			//            are initialized before any access is made to them
		}

		[Preserve]
		internal SKColorSpace (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		protected override void Dispose (bool disposing) =>
			base.Dispose (disposing);

		public bool GammaIsCloseToSrgb =>
			SkiaApi.sk_colorspace_gamma_close_to_srgb (Handle);

		public bool GammaIsLinear =>
			SkiaApi.sk_colorspace_gamma_is_linear (Handle);

		public bool IsSrgb =>
			SkiaApi.sk_colorspace_is_srgb (Handle);

		public SKColorSpaceType Type =>
			SkiaApi.sk_colorspace_gamma_get_type (Handle);

		public SKNamedGamma NamedGamma =>
			SkiaApi.sk_colorspace_gamma_get_gamma_named (Handle);

		public bool IsNumericalTransferFunction =>
			GetNumericalTransferFunction (out _);

		public static bool Equal (SKColorSpace left, SKColorSpace right)
		{
			if (left == null)
				throw new ArgumentNullException (nameof (left));
			if (right == null)
				throw new ArgumentNullException (nameof (right));

			return SkiaApi.sk_colorspace_equals (left.Handle, right.Handle);
		}

		public static SKColorSpace CreateSrgb () => srgb;

		public static SKColorSpace CreateSrgbLinear () => srgbLinear;

		public static SKColorSpace CreateIcc (IntPtr input, long length)
		{
			if (input == IntPtr.Zero)
				throw new ArgumentNullException (nameof (input));

			return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_icc ((void*)input, (IntPtr)length));
		}

		public static SKColorSpace CreateIcc (byte[] input, long length)
		{
			if (input == null)
				throw new ArgumentNullException (nameof (input));

			fixed (byte* i = input) {
				return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_icc (i, (IntPtr)length));
			}
		}

		public static SKColorSpace CreateIcc (byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException (nameof (input));

			fixed (byte* i = input) {
				return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_icc (i, (IntPtr)input.Length));
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use CreateRgb(SKColorSpaceRenderTargetGamma, SKMatrix44) instead.")]
		public static SKColorSpace CreateRgb (SKColorSpaceRenderTargetGamma gamma, SKMatrix44 toXyzD50, SKColorSpaceFlags flags) =>
			CreateRgb (gamma, toXyzD50);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use CreateRgb(SKColorSpaceRenderTargetGamma, SKColorSpaceGamut) instead.")]
		public static SKColorSpace CreateRgb (SKColorSpaceRenderTargetGamma gamma, SKColorSpaceGamut gamut, SKColorSpaceFlags flags) =>
			CreateRgb (gamma, gamut);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use CreateRgb(SKColorSpaceTransferFn, SKMatrix44) instead.")]
		public static SKColorSpace CreateRgb (SKColorSpaceTransferFn coeffs, SKMatrix44 toXyzD50, SKColorSpaceFlags flags) =>
			CreateRgb (coeffs, toXyzD50);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use CreateRgb(SKColorSpaceTransferFn, SKColorSpaceGamut) instead.")]
		public static SKColorSpace CreateRgb (SKColorSpaceTransferFn coeffs, SKColorSpaceGamut gamut, SKColorSpaceFlags flags) =>
			CreateRgb (coeffs, gamut);

		public static SKColorSpace CreateRgb (SKColorSpaceRenderTargetGamma gamma, SKMatrix44 toXyzD50)
		{
			if (toXyzD50 == null)
				throw new ArgumentNullException (nameof (toXyzD50));

			return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_gamma (gamma, toXyzD50.Handle));
		}

		public static SKColorSpace CreateRgb (SKColorSpaceRenderTargetGamma gamma, SKColorSpaceGamut gamut) =>
			GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_gamma_and_gamut (gamma, gamut));

		public static SKColorSpace CreateRgb (SKColorSpaceTransferFn coeffs, SKMatrix44 toXyzD50)
		{
			if (toXyzD50 == null)
				throw new ArgumentNullException (nameof (toXyzD50));

			return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_coeffs (&coeffs, toXyzD50.Handle));
		}

		public static SKColorSpace CreateRgb (SKColorSpaceTransferFn coeffs, SKColorSpaceGamut gamut) =>
			GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_coeffs_and_gamut (&coeffs, gamut));

		public static SKColorSpace CreateRgb (SKNamedGamma gamma, SKMatrix44 toXyzD50)
		{
			if (toXyzD50 == null)
				throw new ArgumentNullException (nameof (toXyzD50));

			return GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_gamma_named (gamma, toXyzD50.Handle));
		}

		public static SKColorSpace CreateRgb (SKNamedGamma gamma, SKColorSpaceGamut gamut) =>
			GetObject<SKColorSpace> (SkiaApi.sk_colorspace_new_rgb_with_gamma_named_and_gamut (gamma, gamut));

		public bool ToXyzD50 (SKMatrix44 toXyzD50)
		{
			if (toXyzD50 == null)
				throw new ArgumentNullException (nameof (toXyzD50));

			return SkiaApi.sk_colorspace_to_xyzd50 (Handle, toXyzD50.Handle);
		}

		public bool GetNumericalTransferFunction (out SKColorSpaceTransferFn fn)
		{
			fixed (SKColorSpaceTransferFn* f = &fn) {
				return SkiaApi.sk_colorspace_is_numerical_transfer_fn (Handle, f);
			}
		}

		public SKMatrix44 ToXyzD50 () =>
			GetObject<SKMatrix44> (SkiaApi.sk_colorspace_as_to_xyzd50 (Handle), false);

		public SKMatrix44 FromXyzD50 () =>
			GetObject<SKMatrix44> (SkiaApi.sk_colorspace_as_from_xyzd50 (Handle), false);

		private sealed class SKColorSpaceStatic : SKColorSpace
		{
			internal SKColorSpaceStatic (IntPtr x)
				: base (x, false)
			{
				IgnorePublicDispose = true;
			}

			protected override void Dispose (bool disposing)
			{
				// do not dispose
			}
		}
	}
}

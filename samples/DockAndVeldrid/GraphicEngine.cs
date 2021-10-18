using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Blazor_Desktop
{
	public class GraphicEngine : IDisposable
	{
		public long FrameTime = 0;
		public long ConvertTime = 0;
		private DateTime? renderTime;

		private readonly GraphicsDevice _graphicsDevice;
		private CommandList _commandList;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		// private Framebuffer _offscreenFrameBuffer;
		private Shader[] _shaders;
		private Pipeline _pipeline;
		private const PixelFormat PIXEL_FORMAT = PixelFormat.B8_G8_R8_A8_UNorm;

		private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";

		private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

		public GraphicEngine(Sdl2Window window)
		{
			GraphicsDeviceOptions options = new GraphicsDeviceOptions
			{
				PreferStandardClipSpaceYDirection = true,
				PreferDepthRangeZeroToOne = true
			};

			_graphicsDevice =
				VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGL);
			CreateResources();
		}

		public void Resize(uint width, uint height) => _graphicsDevice.ResizeMainWindow(width, height);
		
		private VertexPositionColor[] _quadVertices;

		private void CreateResources()
		{
			ResourceFactory factory = _graphicsDevice.ResourceFactory;

			_quadVertices = new[]
			{
				new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
				new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
				new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
				new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow),
			};

			BufferDescription vbDescription = new BufferDescription(
				4 * VertexPositionColor.SizeInBytes,
				BufferUsage.VertexBuffer);
			_vertexBuffer = factory.CreateBuffer(vbDescription);
			
			ushort[] quadIndices = { 0, 1, 2, 3 };
			BufferDescription ibDescription = new BufferDescription(
				4 * sizeof(ushort),
				BufferUsage.IndexBuffer);
			_indexBuffer = factory.CreateBuffer(ibDescription);

			_graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _quadVertices);
			_graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

			VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
					VertexElementFormat.Float2),
				new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
					VertexElementFormat.Float4));

			ShaderDescription vertexShaderDesc = new ShaderDescription(
				ShaderStages.Vertex,
				Encoding.UTF8.GetBytes(VertexCode),
				"main");
			ShaderDescription fragmentShaderDesc = new ShaderDescription(
				ShaderStages.Fragment,
				Encoding.UTF8.GetBytes(FragmentCode),
				"main");

			_shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

			CreatePipeline(vertexLayout, _graphicsDevice.SwapchainFramebuffer);

			_commandList = factory.CreateCommandList();
		}

		private void CreatePipeline(VertexLayoutDescription vertexLayout, Framebuffer frameBuffer)
		{
			GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
			pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
			pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
				depthTestEnabled: true,
				depthWriteEnabled: true,
				comparisonKind: ComparisonKind.LessEqual);
			pipelineDescription.RasterizerState = new RasterizerStateDescription(
				cullMode: FaceCullMode.Back,
				fillMode: PolygonFillMode.Solid,
				frontFace: FrontFace.Clockwise,
				depthClipEnabled: true,
				scissorTestEnabled: false);
			pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
			pipelineDescription.ShaderSet = new ShaderSetDescription(
				vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
				shaders: _shaders);
			if (frameBuffer is object)
				pipelineDescription.Outputs = frameBuffer.OutputDescription;

			_pipeline = _graphicsDevice.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
		}

		private void RedrawContent(double deltaTime)
		{
			var theta = deltaTime * Math.PI;
			for (int i = 0; i < _quadVertices.Length; i++)
			{
				var p = _quadVertices[i].Position;
				// var x = Math.Clamp(p.X * (float)Math.Cos(theta) - p.Y * (float)Math.Sin(theta), -1, 1);
				var x =p.X * (float)Math.Cos(theta) - p.Y * (float)Math.Sin(theta);
				var y = Math.Clamp(p.X * (float)Math.Sin(theta) + p.Y * (float)Math.Cos(theta), -1, 1);
				_quadVertices[i].Position = new Vector2(x, y);
			}

			_graphicsDevice.UpdateBuffer(_vertexBuffer, 0, _quadVertices);
			_commandList.ClearColorTarget(0, RgbaFloat.Black);
			// Set all relevant state to draw our quad.
			_commandList.SetVertexBuffer(0, _vertexBuffer);
			_commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			_commandList.SetPipeline(_pipeline);
			// Issue a Draw command for a single instance with 4 indices.
			_commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
		}
		
		public void Render()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			_commandList.Begin();
			_commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
			if (renderTime is null)
				renderTime = DateTime.Now;
			var deltaTime = (DateTime.Now - renderTime).Value.TotalMilliseconds / 1000;
			RedrawContent(deltaTime);
			_commandList.End();
			_graphicsDevice.SubmitCommands(_commandList);
			_graphicsDevice.SwapBuffers();
			FrameTime = sw.ElapsedMilliseconds;
			renderTime = DateTime.Now;
		}

		private void DisposeResources()
		{
			_pipeline.Dispose();
			foreach (Shader shader in _shaders)
			{
				shader.Dispose();
			}

			_commandList.Dispose();
			_vertexBuffer.Dispose();
			_indexBuffer.Dispose();
			_graphicsDevice.Dispose();
		}

		public void Dispose()
		{
			DisposeResources();
		}
	}

	struct VertexPositionColor
	{
		public const uint SizeInBytes = 24;
		public Vector2 Position;
		public RgbaFloat Color;

		public VertexPositionColor(Vector2 position, RgbaFloat color)
		{
			Position = position;
			Color = color;
		}
	}
}

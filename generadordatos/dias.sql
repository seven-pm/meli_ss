SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[dias](
	[diaId] [numeric](18, 0) NOT NULL,
	[tipoClimaId] [numeric](18, 0) NOT NULL,
 CONSTRAINT [PK_dias] PRIMARY KEY CLUSTERED 
(
	[diaId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[dias]  WITH CHECK ADD  CONSTRAINT [FK_dias_tipos_clima] FOREIGN KEY([tipoClimaId])
REFERENCES [dbo].[tipos_clima] ([tipoClimaId])
GO

ALTER TABLE [dbo].[dias] CHECK CONSTRAINT [FK_dias_tipos_clima]
GO

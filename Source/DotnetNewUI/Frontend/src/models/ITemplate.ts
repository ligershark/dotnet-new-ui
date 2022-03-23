export default interface ITemplate {
  readonly PackageName: string;
  readonly base64Icon: string;
  readonly templateManifest: ITemplateManifest;
}

export interface ITemplateManifest {
  identity: string;
  name: string;
  author: string;
  classifications: string[];
  description: string;
  shortName: string[];
  tags: ITemplateTags;
}

export interface ITemplateTags {
  language: string;
  type: string;
}

export default interface ITemplate {
  readonly packageName: string;
  readonly version: string;
  readonly base64Icon: string;
  readonly isBuiltIn: boolean;
  readonly templateManifest: ITemplateManifest;
  readonly ideHostManifest: IIdeHostManifest;
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

export interface IIdeHostManifest {
  icon: string;
  learnMoreLink: string;
}

export interface ITemplateTags {
  language: string;
  type: string;
}

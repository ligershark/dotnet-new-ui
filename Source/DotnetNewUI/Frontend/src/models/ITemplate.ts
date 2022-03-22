export default interface ITemplate {
  readonly authors: string[];
  readonly description: string;
  readonly iconUrl: string;
  readonly id: string;
  readonly owners: Array<string>;
  readonly projectUrl: string;
  readonly summary: string;
  readonly tags: Array<string>;
  readonly title: string;
  readonly totalDownloads: number;
  readonly verified: boolean;
  readonly version: string;
}

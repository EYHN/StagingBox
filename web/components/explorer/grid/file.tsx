import { memo } from 'react';
import { IFileFragment } from 'api';
import FileIcon from 'components/file-icons';
import styled from '@emotion/styled';

interface FileProps {
  file: IFileFragment;
  height: number;
  width: number;
  focus: boolean;
  className?: string;
  style?: React.CSSProperties;
  onDoubleClick?: React.MouseEventHandler;
  onMouseDown?: React.MouseEventHandler;
}

const topPadding = 8;
const bottomPadding = 10;
const leftPadding = 4;
const rightPadding = 4;
const textHeight = 75;

const Container = styled.div({
  position: 'relative',
  padding: `${topPadding}px ${rightPadding}px ${bottomPadding}px ${leftPadding}px`,
});

const TextContainer = styled.div({
  width: '100%',
  height: textHeight,
  paddingTop: '12px',
});

const FileName = styled.p(({ theme }) => ({
  display: '-webkit-box',
  WebkitLineClamp: 2,
  WebkitBoxOrient: 'vertical',
  lineHeight: '1.5',
  fontSize: '14px',
  textOverflow: 'ellipsis',
  overflowWrap: 'break-word',
  overflow: 'hidden',
  margin: 0,
  textAlign: 'center',
  color: theme.colors.gray100,
}));

const File: React.FunctionComponent<FileProps> = memo(({ file, width, height, className, style, onDoubleClick, onMouseDown }) => {
  const imageSize = Math.min(height - topPadding - bottomPadding - textHeight, width - leftPadding - rightPadding);
  const imageLeft = (width - leftPadding - rightPadding - imageSize) / 2;
  const textTopMargin = height - topPadding - bottomPadding - textHeight - imageSize;

  return (
    <Container className={className} style={{ width, height, ...style }}>
      <FileIcon
        file={file}
        width={imageSize}
        height={imageSize}
        style={{ marginLeft: imageLeft, marginRight: imageLeft }}
        onDoubleClick={onDoubleClick}
        onMouseDown={onMouseDown}
      />
      <TextContainer style={{ marginTop: textTopMargin }}>
        <FileName>{file.name}</FileName>
      </TextContainer>
    </Container>
  );
});

File.displayName = 'File';

export default File;

import styled from '@emotion/styled';

const LEFT_SIZE = 240;
const RIGHT_SIZE = 240;

const Container = styled.div({
  width: '100vw',
  height: '100vh',
  overflow: 'hidden',
});

const LeftContainer = styled.div(({ theme }) => ({
  display: 'inline-block',
  background: theme.colors.secondBackground,
  overflow: 'hidden',
  height: '100%',
  width: LEFT_SIZE,
}));

const RightContainer = styled.div(({ theme }) => ({
  display: 'inline-block',
  background: theme.colors.secondBackground,
  overflow: 'hidden',
  height: '100%',
  width: LEFT_SIZE,
}));

const CenterContainer = styled.div(({ theme }) => ({
  display: 'inline-block',
  background: theme.colors.background,
  overflow: 'hidden',
  height: '100%',
  width: `calc(100% - ${LEFT_SIZE}px - ${RIGHT_SIZE}px)`,
}));

const ToolBarContainer = styled.div({
  marginTop: '8px',
  height: '92px',
  overflow: 'hidden',
});

const ExplorerContainer = styled.div({
  height: 'calc(100% - 100px)',
  width: '100%',
});

interface IAppLayoutProps {
  left: React.ReactNode;
  tooltip: React.ReactNode;
  explorer: React.ReactNode;
  right: React.ReactNode;
}

const AppLayout: React.FunctionComponent<IAppLayoutProps> = ({ left, tooltip, explorer, right }) => {
  return (
    <Container>
      <LeftContainer>{left}</LeftContainer>
      <CenterContainer>
        <ToolBarContainer>{tooltip}</ToolBarContainer>
        <ExplorerContainer>{explorer}</ExplorerContainer>
      </CenterContainer>
      <RightContainer>{right}</RightContainer>
    </Container>
  );
};

export default AppLayout;
